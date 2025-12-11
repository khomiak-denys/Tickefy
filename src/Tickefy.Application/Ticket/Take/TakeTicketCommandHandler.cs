using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.ActivityLog;
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
        private readonly IUserRepository _useRepository;
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
            _useRepository = userRepository;
            _uow = uow;
        }
        public async Task<Unit> Handle(TakeTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);

            if (ticket.Status != Status.Created) throw new ForbiddenException("Ticket already assigned");

            var user = await _useRepository.GetByIdAsync(command.UserId);
            if (user == null) throw new NotFoundException(nameof(user), command.UserId);
            if (user.TeamId is null) throw new ForbiddenException("You should be in team to take tickets");

            var isAgent = command.Roles.Contains(UserRoles.Agent.ToString()) && (ticket.AssignedAgentId is null);
            if (!isAgent) throw new ForbiddenException("Obtain agent role to take tickets");

            
            
            var team = await _teamRepository.GetByIdAsync(user.TeamId);
            if (team == null) throw new NotFoundException(nameof(team), user.TeamId);

            ticket.Assign(user.Id, team.Id);

            var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, user.Id, Domain.Common.Event.EventType.UserAssigned, "Agent assigned");

            await _uow.SaveChangesAsync();
            
            return Unit.Value;
        }
    }
}
