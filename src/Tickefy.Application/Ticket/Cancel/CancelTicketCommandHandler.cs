using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Cancel
{
    public class CancelTicketCommandHandler : ICommandHandler<CancelTicketCommand>
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
        public async Task Handle(CancelTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);

            var isRequester = command.Roles.Contains(UserRoles.Requester.ToString()) && (ticket.RequesterId.Value == command.UserId.Value);
            var isAdmin = command.Roles.Contains(UserRoles.Admin.ToString());

            if (isAdmin || isRequester)
            {
                if (ticket.Status == Status.Created)
                {
                    ticket.Cancel();
                    var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged, "Ticket canceled");
                    _logRepository.Add(log);
                    await _uow.SaveChangesAsync();
                }
                else
                {
                    throw new ForbiddenException($"Ticket status must be 'Created' to cancel. Current status {ticket.Status}");
                }
            }
            else
            {
                throw new ForbiddenException("Only admin or requester can cancel tickets");
            }
        }
    }
}
