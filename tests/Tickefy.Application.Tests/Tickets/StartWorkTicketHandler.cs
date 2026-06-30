using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Ticket.StartWork;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tests.Tickets;

public class StartWorkTicketHandlerTests
{
    [Fact]
    public async Task StartWorkTicketCommandHandler_Should_Return_NotFoundError_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Tickets.Ticket?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new StartWorkTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new StartWorkTicketCommand
        {
            UserId = new UserId(),
            Roles = new List<string>(),
            TicketId = new TicketId()
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task StartWorkTicketCommandHandler_Should_Return_ForbiddenError_On_RequesterRole()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Tickets.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow));

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new StartWorkTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new StartWorkTicketCommand
        {
            UserId = new UserId(),
            Roles = [nameof(UserRoles.Requester)],
            TicketId = new TicketId()
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ForbiddenError>();
    }

    [Fact]
    public async Task StartWorkTicketCommandHandler_Should_StartWorkOnTicketAndLogReason_WhenConditionAllows()
    {
        var ticket = Domain.Tickets.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);
        var ticketId = ticket.Id;
        var agentId = new UserId();
        var eventType = EventType.StatusChanged;

        ticket.Take(agentId, new TeamId());

        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new StartWorkTicketCommandHandler(repo.Object, logRepository.Object, uow.Object);

        var command = new StartWorkTicketCommand
        {
            UserId = agentId,
            Roles = [nameof(UserRoles.Agent)],
            TicketId = ticketId
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        logRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLogs.ActivityLog>(log =>
            log.UserId == agentId &&
            log.TicketId == ticketId &&
            log.EventType == eventType
        )), Times.Once);

        uow.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
