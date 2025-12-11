using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Complete
{
    public class CompleteTicketCommandHandler : ICommandHandler<CompleteTicketCommand, Unit>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IUnitOfWork _uow;

        public CompleteTicketCommandHandler(
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IUnitOfWork uow)
        {
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _uow = uow;
        }
        public async Task<Unit> Handle(CompleteTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);

            var isAssignedAgent = command.Roles.Contains(UserRoles.Agent.ToString()) && (ticket.AssignedAgentId?.Value == command.UserId.Value); 
            var isAdmin = command.Roles.Contains(UserRoles.Admin.ToString());

            if (isAdmin || isAssignedAgent)
            {
                if (ticket.Status == Status.Assigned)
                {
                    ticket.Complete();
                    var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged, "Ticket completed");
                    _logRepository.Add(log);
                    await _uow.SaveChangesAsync();
                }
                else
                {
                    throw new ForbiddenException($"Ticket status must be 'Assigned' to complete. Current status {ticket.Status}");
                }
            }
            else
            {
                throw new ForbiddenException("Only admin or assigned agent can complete tickets");
            }

            return Unit.Value;
        }   
    }
}
