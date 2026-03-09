using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Ticket.Cancel;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Tests.Tickets;

public class CancelTicketHandler
{
    [Fact]
    public async Task CancelTicketCommandHandler_Should_Return_NotFoundError_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Ticket.Ticket?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new CancelTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new CancelTicketCommand(new UserId(), new List<string>(), new TicketId(), string.Empty);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }
    
    [Fact]
    public async Task CancelTicketCommandHandler_Should_Return_ForbiddenError_On_AgentRole()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow));

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new CancelTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new CancelTicketCommand(new UserId(), [nameof(UserRoles.Agent)], new TicketId(), string.Empty);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ForbiddenError>();
    }

    [Fact]
    public async Task CancelTicketCommandHandler_Should_CancelTicketAndLogReason_WhenConditionAllows()
    {
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);
        var ticketId = ticket.Id;
        var userId = new UserId();
        var eventType = EventType.StatusChanged;
        var reason = "Test description";
        
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        
        var handler = new CancelTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new CancelTicketCommand(userId, [nameof(UserRoles.Admin)], ticketId, reason);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        logRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLog.ActivityLog>(log =>
            log.UserId == userId &&
                log.TicketId == ticketId &&
                log.EventType == eventType &&
                log.Description.Contains(reason)
        )), Times.Once);
        
        uow.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
