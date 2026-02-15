using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Cancel;
using Tickefy.Application.Ticket.Revise;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Tests.Tickets;

public class ReopenTicketHandler
{
     [Fact]
    public async Task ReopenTicketCommandHandler_Should_Throw_NotFoundException_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Ticket.Ticket?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new ReopenTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new ReopenTicketCommand {
            UserId = new UserId(), 
            Roles = new List<string>(), 
            TicketId = new TicketId(), 
            Reason = string.Empty
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task ReopenTicketCommandHandler_Should_Throw_ForBiddenException_On_AgentRole()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow));

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new ReopenTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new ReopenTicketCommand {
            UserId = new UserId(), 
            Roles = ["Agent"], 
            TicketId = new TicketId(), 
            Reason = string.Empty
            
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task ReopenTicketCommandHandler_Should_ReopenTicketAndLogReason_WhenConditionAllows()
    {
        var userId = new UserId();
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, userId, DateTime.UtcNow);
        var ticketId = ticket.Id;
        var eventType = EventType.StatusChanged;
        var reason = "Test description";
        
        ticket.Take(new UserId(), new TeamId());
        ticket.StartWork();
        ticket.Complete();
        
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new ReopenTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new ReopenTicketCommand {
            UserId = userId, 
            Roles = ["Requester"], 
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