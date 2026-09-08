using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common.Helpers;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Tickets;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tickets.Take
{
    public class TakeTicketCommandHandler : ICommandHandler<TakeTicketCommand, Result>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<TakeTicketCommandHandler> _logger;

        public TakeTicketCommandHandler(
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            IUnitOfWork uow,
            ILogger<TakeTicketCommandHandler> logger)
        {
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result> Handle(TakeTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found when attempting to take ticket", command.TicketId.Value);
                return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
            }

            if (!TicketAction.Take.CanExecute(ticket, command.UserId, command.Roles))
            {
                _logger.LogWarning("User {UserId} with roles {Roles} forbidden from taking ticket {TicketId}", command.UserId.Value, command.Roles, command.TicketId.Value);
                return Result.Failure(new ForbiddenError("Access denied"));
            }

            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when attempting to take ticket {TicketId}", command.UserId.Value, command.TicketId.Value);
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId));
            }

            if (user.TeamId is null)
            {
                _logger.LogWarning("User {UserId} is not assigned to a team and cannot take ticket {TicketId}", command.UserId.Value, command.TicketId.Value);
                return Result.Failure(new ForbiddenError("You should be in team to take tickets"));
            }

            var team = await _teamRepository.GetByIdAsync(user.TeamId, cancellationToken);
            if (team == null)
            {
                _logger.LogWarning("Team {TeamId} not found for user {UserId} when taking ticket {TicketId}", user.TeamId?.Value, command.UserId.Value, command.TicketId.Value);
                return Result.Failure(new NotFoundError(nameof(team) + " " + user.TeamId));
            }

            ticket.Take(user.Id, team.Id);

            var log = Domain.ActivityLogs.ActivityLog.Create(
                ticket.Id,
                user.Id,
                Domain.Common.Event.EventType.UserAssigned,
                "Agent assigned");
            _logRepository.Add(log);
            await _uow.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Ticket {TicketId} successfully taken by agent {UserId} in team {TeamId}", ticket.Id.Value, user.Id.Value, team.Id.Value);

            return Result.Success();
        }
    }
}
