using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.Cancel
{
    public class CancelTicketCommand : ICommand
    {
        public UserId UserId { get; init; }
        public List<string> Roles { get; init; }
        public TicketId TicketId { get; init; }
    }
}
