using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Accept;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Tests.Tickets;

public class AcceptTicketHandlerTests
{
    [Fact]
    public async Task AcceptTicketCommandHandler_Should_Throw_NotFoundException_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Ticket.Ticket?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new AcceptTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new AcceptTicketCommand{
            UserId = new UserId(), 
            Roles = new List<string>(), 
            TicketId = new TicketId()
        };
        
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task AcceptTicketCommandHandler_Should_Throw_ForBiddenException_On_NonRequesterRole()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow));

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new AcceptTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new AcceptTicketCommand {
            UserId = new UserId(), 
            Roles = [nameof(UserRoles.Requester)], 
            TicketId = new TicketId()
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<ForbiddenException>();
    }
    
    [Fact]
    public async Task AcceptTicketCommandHandler_Should_AcceptTicket_WhenConditionAllows()
    {
        var requesterId = new UserId();
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, requesterId, DateTime.UtcNow);
        var ticketId = ticket.Id;
        var eventType = EventType.StatusChanged;
        
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();
        
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new AcceptTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new AcceptTicketCommand {
            UserId = requesterId, 
            Roles = [nameof(UserRoles.Agent)], 
            TicketId = ticketId
        };
        await handler.Handle(command, CancellationToken.None);

        logRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLog.ActivityLog>(log =>
            log.UserId == requesterId &&
            log.TicketId == ticketId &&
            log.EventType == eventType 
        )), Times.Once);
        
        uow.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}