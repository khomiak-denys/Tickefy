using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Fail;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Tests.Tickets;

public class FailTicketHandlerTests
{
    [Fact]
    public async Task FailTicketCommandHandler_Should_Throw_NotFoundException_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Ticket.Ticket?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new FailTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new FailTicketCommand {
            UserId = new UserId(), 
            Roles = new List<string>(), 
            TicketId = new TicketId(), 
            Reason = string.Empty
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task FailTicketCommandHandler_Should_Throw_ForBiddenException_On_AgentOrRequesterRole()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow));

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new FailTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new FailTicketCommand {
            UserId = new UserId(), 
            Roles = [nameof(UserRoles.Agent), nameof(UserRoles.Requester)], 
            TicketId = new TicketId(), 
            Reason = string.Empty
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task FailTicketCommandHandler_Should_CancelTicketAndLogReason_WhenConditionAllows()
    {
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        
        var ticketId = ticket.Id;
        var userId = new UserId();
        var eventType = EventType.StatusChanged;
        var reason = "Test description";
        
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new FailTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new FailTicketCommand {
            UserId = userId, 
            Roles = [nameof(UserRoles.Admin)], 
            TicketId = ticketId, 
            Reason = reason
        };
        await handler.Handle(command, CancellationToken.None);

        logRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLog.ActivityLog>(log =>
            log.UserId == userId &&
                log.TicketId == ticketId &&
                log.EventType == eventType &&
                log.Description.Contains(reason)
        )), Times.Once);
        
        uow.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}