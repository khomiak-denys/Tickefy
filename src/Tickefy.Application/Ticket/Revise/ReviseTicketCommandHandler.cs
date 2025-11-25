using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Revise
{
    public class ReviseTicketCommandHandler : ICommandHandler<ReviseTicketCommand>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IUnitOfWork _uow;

        public ReviseTicketCommandHandler(
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IUnitOfWork uow)
        {
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _uow = uow;
        }
        public async Task Handle(ReviseTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);

            var isRequester = command.Roles.Contains(UserRoles.Requester.ToString()) && (ticket.RequesterId.Value == command.UserId.Value);
            var isAdmin = command.Roles.Contains(UserRoles.Admin.ToString());

            if (isAdmin || isRequester)
            {
                if (ticket.Status == Status.Completed)
                {
                    ticket.Revise();
                    var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged, "Ticket revised");
                    _logRepository.Add(log);
                    await _uow.SaveChangesAsync();
                }
                else
                {
                    throw new ForbiddenException($"Ticket status must be 'Completed' to revise. Current status {ticket.Status}");
                }
            }
            else
            {
                throw new ForbiddenException("Only admin or requester agent can revise tickets");
            }
        }
    }
}
