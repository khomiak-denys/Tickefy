using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Revise
{
    public class ReopenTicketCommandHandler : ICommandHandler<ReopenTicketCommand, Unit>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IUnitOfWork _uow;

        public ReopenTicketCommandHandler(
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IUnitOfWork uow)
        {
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _uow = uow;
        }
        public async Task<Unit> Handle(ReopenTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);
            
            if (TicketAction.Reopen.CanExecute(ticket, command.UserId, command.Roles))
            {
                ticket.Reopen();
                var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged,
                    $"Ticket reopened.Reason: {command.Reason}");
                _logRepository.Add(log);
                await _uow.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new ForbiddenException("Only admin or requester agent can revise tickets");
            }
            return Unit.Value;
        }
    }
}
