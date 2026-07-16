using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Tickets.Revise;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tests.Tickets;

public class ReopenTicketHandler
{
    [Fact]
    public async Task ReopenTicketCommandHandler_Should_Return_NotFoundError_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Tickets.Ticket?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new ReopenTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new ReopenTicketCommand
        {
            UserId = new UserId(),
            Roles = new List<string>(),
            TicketId = new TicketId(),
            Reason = string.Empty
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task ReopenTicketCommandHandler_Should_Return_ForbiddenError_On_AgentRole()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Tickets.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow));

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new ReopenTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new ReopenTicketCommand
        {
            UserId = new UserId(),
            Roles = [nameof(UserRoles.Agent)],
            TicketId = new TicketId(),
            Reason = string.Empty
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ForbiddenError>();
    }

    [Fact]
    public async Task ReopenTicketCommandHandler_Should_ReopenTicketAndLogReason_WhenConditionAllows()
    {
        var userId = new UserId();
        var ticket = Domain.Tickets.Ticket.Create(string.Empty, string.Empty, userId, DateTime.UtcNow);
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

        var command = new ReopenTicketCommand
        {
            UserId = userId,
            Roles = [nameof(UserRoles.Requester)],
            TicketId = ticketId,
            Reason = reason
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        logRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLogs.ActivityLog>(log =>
            log.UserId == userId &&
            log.TicketId == ticketId &&
            log.EventType == eventType &&
            log.Description.Contains(reason)
        )), Times.Once);

        uow.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
