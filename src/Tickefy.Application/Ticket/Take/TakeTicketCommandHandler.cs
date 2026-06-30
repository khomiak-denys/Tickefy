using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Team;
using Tickefy.Domain.Ticket;
using Tickefy.Domain.User;

namespace Tickefy.Application.Ticket.Take
{
    public class TakeTicketCommandHandler : ICommandHandler<TakeTicketCommand, Result>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;

        public TakeTicketCommandHandler(
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            IUnitOfWork uow)
        {
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _uow = uow;
        }

        public async Task<Result> Handle(TakeTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null)
            {
                return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
            }

            if (!TicketAction.Take.CanExecute(ticket, command.UserId, command.Roles))
            {
                return Result.Failure(new ForbiddenError("Access denied"));
            }

            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId));
            }

            if (user.TeamId is null)
            {
                return Result.Failure(new ForbiddenError("You should be in team to take tickets"));
            }

            var team = await _teamRepository.GetByIdAsync(user.TeamId, cancellationToken);
            if (team == null)
            {
                return Result.Failure(new NotFoundError(nameof(team) + " " + user.TeamId));
            }

            ticket.Take(user.Id, team.Id);

            var log = Domain.ActivityLog.ActivityLog.Create(
                ticket.Id,
                user.Id,
                Domain.Common.Event.EventType.UserAssigned,
                "Agent assigned");
            _logRepository.Add(log);
            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
