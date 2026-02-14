using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Cancel
{
    public class CancelTicketCommandHandler : ICommandHandler<CancelTicketCommand, Unit>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IUnitOfWork _uow;

        public CancelTicketCommandHandler(
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IUnitOfWork uow)
        {
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _uow = uow;
        }
        public async Task<Unit> Handle(CancelTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);

            if (TicketAction.Cancel.CanExecute(ticket, command.UserId, command.Roles))
            {
                    ticket.Cancel();
                    var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged, $"Ticket canceled. Reason: {command.Reason}");
                    _logRepository.Add(log);
                    await _uow.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new ForbiddenException("Only admin or requester can cancel tickets");
            }

            return Unit.Value;
        }
    }
}