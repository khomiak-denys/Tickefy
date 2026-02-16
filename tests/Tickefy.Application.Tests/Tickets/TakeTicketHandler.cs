using Moq;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Cancel;
using Tickefy.Application.Ticket.Take;
using Tickefy.Domain.ActivityLog;
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
    public async Task TakeTicketCommandHandler_Should_Throw_NotFoundException_On_Null_Ticket()
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

        var command = new TakeTicketCommand {
            UserId = new UserId(), 
            Roles = new List<string>(), 
            TicketId = new TicketId()
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task TakeTicketCommandHandler_Should_Throw_ForbiddenException_On_RequesterRole()
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

        var command = new TakeTicketCommand {
            UserId = new UserId(), 
            Roles = [nameof(UserRoles.Requester)], 
            TicketId = new TicketId()
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task TakeTicketCommandHandler_Should_Throw_NotFoundException_On_Null_Requester()
    {
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);
        
        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<UserId>()))
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

        var command = new TakeTicketCommand {
            UserId = new UserId(), 
            Roles = [nameof(UserRoles.Agent)], 
            TicketId = new TicketId()
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task TakeTicketCommandHandler_Should_Throw_NotFoundException_When_Requester_TeamId_IsNull()
    {
        var user = Domain.User.User.Create(string.Empty, string.Empty, string.Empty, string.Empty);
        var team = Domain.Team.Team.Create(string.Empty, string.Empty);
        
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);
        
        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<UserId>()))
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

        var command = new TakeTicketCommand {
            UserId = new UserId(), 
            Roles = [nameof(UserRoles.Agent)], 
            TicketId = new TicketId()
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task TakeTicketCommandHandler_Should_AssignTicket_And_Log()
    {
        var user = Domain.User.User.Create(string.Empty, string.Empty, string.Empty, string.Empty);
        var team = Domain.Team.Team.Create(string.Empty, string.Empty);
        var teamId = team.Id;
        
        var ticket = Domain.Ticket.Ticket.Create(string.Empty, string.Empty, new UserId(), DateTime.UtcNow);
        
        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository.Setup(r => r.GetByIdAsync(It.IsAny<TicketId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(It.IsAny<UserId>()))
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

        var command = new TakeTicketCommand {
            UserId = new UserId(), 
            Roles = [nameof(UserRoles.Agent)], 
            TicketId = new TicketId()
        };
        var func = () => handler.Handle(command, CancellationToken.None);
        
        await func.Should().ThrowAsync<NotFoundException>();
    } 
}