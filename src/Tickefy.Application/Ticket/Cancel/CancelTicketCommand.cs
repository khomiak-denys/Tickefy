using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.Cancel
{
    public class CancelTicketCommand : ICommand<Result>
    {
        public UserId UserId { get; init; }
        public IEnumerable<string> Roles { get; init; }
        public TicketId TicketId { get; init; }
        public string Reason { get; init; }

        public CancelTicketCommand(
            UserId userId,
            IEnumerable<string> roles,
            TicketId ticketId,
            string reason
        )
        {
            UserId = userId;
            Roles = roles;
            TicketId = ticketId;
            Reason = reason;
        }
    }
}
