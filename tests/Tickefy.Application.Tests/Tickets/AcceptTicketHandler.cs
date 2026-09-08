using Microsoft.Extensions.Logging;
using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Tickets.Accept;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tests.Tickets;

public class AcceptTicketHandlerTests
{
    [Fact]
    public async Task AcceptTicketCommandHandler_Should_Return_NotFoundError_On_Null_Ticket()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Tickets.Ticket?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<AcceptTicketCommandHandler>>();

        var handler = new AcceptTicketCommandHandler(repo.Object, logRepository.Object, uow.Object, logger.Object);

        var command = new AcceptTicketCommand
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
    public async Task AcceptTicketCommandHandler_Should_Return_ForbiddenError_On_NonRequesterRole()
    {
        var repo = new Mock<ITicketRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Tickets.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow));

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<AcceptTicketCommandHandler>>();

        var handler = new AcceptTicketCommandHandler(repo.Object, logRepository.Object, uow.Object, logger.Object);

        var command = new AcceptTicketCommand
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
    public async Task AcceptTicketCommandHandler_Should_AcceptTicket_WhenConditionAllows()
    {
        var requesterId = new UserId();
        var ticket = Domain.Tickets.Ticket.Create(string.Empty, string.Empty, requesterId, DateTime.UtcNow);
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
        var logger = new Mock<ILogger<AcceptTicketCommandHandler>>();

        var handler = new AcceptTicketCommandHandler(repo.Object, logRepository.Object, uow.Object, logger.Object);

        var command = new AcceptTicketCommand
        {
            UserId = requesterId,
            Roles = [nameof(UserRoles.Agent)],
            TicketId = ticketId
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        logRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLogs.ActivityLog>(log =>
            log.UserId == requesterId &&
            log.TicketId == ticketId &&
            log.EventType == eventType
        )), Times.Once);

        uow.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
