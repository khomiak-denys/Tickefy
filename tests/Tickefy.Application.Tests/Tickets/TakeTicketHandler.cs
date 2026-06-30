using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Ticket.Take;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Team;
using Tickefy.Domain.Ticket;
using Tickefy.Domain.User;

namespace Tickefy.Application.Tests.Tickets;

public class TakeTicketHandler
{
    [Fact]
    public async Task TakeTicketCommandHandler_Should_Return_NotFoundError_On_Null_Ticket()
    {
        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Ticket.Ticket?)null);

        var userRepository = new Mock<IUserRepository>();
        var teamRepository = new Mock<ITeamRepository>();

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new TakeTicketCommandHandler(
            ticketRepository.Object,
            logRepository.Object,
            userRepository.Object,
            teamRepository.Object,
            uow.Object);

        var command = new TakeTicketCommand
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
    public async Task TakeTicketCommandHandler_Should_Return_ForbiddenError_On_RequesterRole()
    {
        var requesterId = new UserId();

        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, requesterId, DateTime.UtcNow);

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        var teamRepository = new Mock<ITeamRepository>();

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new TakeTicketCommandHandler(
            ticketRepository.Object,
            logRepository.Object,
            userRepository.Object,
            teamRepository.Object,
            uow.Object);

        var command = new TakeTicketCommand
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
    public async Task TakeTicketCommandHandler_Should_Return_NotFoundError_On_Null_Agent()
    {
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.User.User?)null);
        var teamRepository = new Mock<ITeamRepository>();

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new TakeTicketCommandHandler(
            ticketRepository.Object,
            logRepository.Object,
            userRepository.Object,
            teamRepository.Object,
            uow.Object);

        var command = new TakeTicketCommand
        {
            UserId = new UserId(),
            Roles = [nameof(UserRoles.Agent)],
            TicketId = new TicketId()
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task TakeTicketCommandHandler_Should_Return_ForbiddenError_When_Agent_TeamId_IsNull()
    {
        var user = Domain.User.User.Create(string.Empty, string.Empty, string.Empty, string.Empty);

        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var teamRepository = new Mock<ITeamRepository>();

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new TakeTicketCommandHandler(
            ticketRepository.Object,
            logRepository.Object,
            userRepository.Object,
            teamRepository.Object,
            uow.Object);

        var command = new TakeTicketCommand
        {
            UserId = new UserId(),
            Roles = [nameof(UserRoles.Agent)],
            TicketId = new TicketId()
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ForbiddenError>();
    }

    [Fact]
    public async Task TakeTicketCommandHandler_Should_Return_NotFoundError_When_Agent_Team_IsNotFound()
    {
        var user = Domain.User.User.Create(string.Empty, string.Empty, string.Empty, string.Empty);
        var team = Domain.Team.Team.Create(string.Empty, string.Empty);

        team.AddMember(user);

        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var teamRepository = new Mock<ITeamRepository>();
        teamRepository.Setup(r => r.GetByIdAsync(It.IsAny<TeamId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Team.Team?)null);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new TakeTicketCommandHandler(
            ticketRepository.Object,
            logRepository.Object,
            userRepository.Object,
            teamRepository.Object,
            uow.Object);

        var command = new TakeTicketCommand
        {
            UserId = user.Id,
            Roles = [nameof(UserRoles.Agent)],
            TicketId = new TicketId()
        };
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task TakeTicketCommandHandler_Should_AssignTicket_And_Log()
    {
        var user = Domain.User.User.Create(string.Empty, string.Empty, string.Empty, string.Empty);
        var team = Domain.Team.Team.Create(string.Empty, string.Empty);
        team.AddMember(user);

        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var teamRepository = new Mock<ITeamRepository>();
        teamRepository.Setup(r => r.GetByIdAsync(It.IsAny<TeamId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var logRepository = new Mock<IActivityLogRepository>();
        var uow = new Mock<IUnitOfWork>();

        var handler = new TakeTicketCommandHandler(
            ticketRepository.Object,
            logRepository.Object,
            userRepository.Object,
            teamRepository.Object,
            uow.Object);

        var command = new TakeTicketCommand
        {
            UserId = user.Id,
            Roles = [nameof(UserRoles.Agent)],
            TicketId = new TicketId()
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        logRepository.Verify(repo => repo.Add(It.Is<Domain.ActivityLog.ActivityLog>(log =>
            log.UserId == user.Id &&
            log.TicketId == ticket.Id &&
            log.EventType == EventType.UserAssigned &&
            log.Description != string.Empty
        )));

        uow.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
