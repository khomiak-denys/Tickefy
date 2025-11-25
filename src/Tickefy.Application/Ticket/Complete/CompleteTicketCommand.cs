using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.Complete
{
    public class CompleteTicketCommand : ICommand
    {
        public UserId UserId {  get; init; }
        public List<string> Roles { get; init; }
        public TicketId TicketId { get; init; }
    }
}
