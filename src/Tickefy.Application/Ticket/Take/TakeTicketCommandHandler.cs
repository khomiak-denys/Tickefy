using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Team;
using Tickefy.Domain.Ticket;
using Tickefy.Domain.User;

namespace Tickefy.Application.Ticket.Take
{
    public class TakeTicketCommandHandler : ICommandHandler<TakeTicketCommand, Unit>
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
        public async Task<Unit> Handle(TakeTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);

            if (TicketAction.Take.CanExecute(ticket, command.UserId, command.Roles))
            {
                var user = await _userRepository.GetByIdAsync(command.UserId);
                if (user == null) throw new NotFoundException(nameof(user), command.UserId);
                if (user.TeamId is null) throw new ForbiddenException("You should be in team to take tickets");
                
                var team = await _teamRepository.GetByIdAsync(user.TeamId);
                if (team == null) throw new NotFoundException(nameof(team), user.TeamId);

                ticket.Take(user.Id, team.Id);

                var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, user.Id,
                    Domain.Common.Event.EventType.UserAssigned, "Agent assigned");
                _logRepository.Add(log);
                await _uow.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new ForbiddenException("Access denied");
            }

            return Unit.Value;
        }
    }
}
