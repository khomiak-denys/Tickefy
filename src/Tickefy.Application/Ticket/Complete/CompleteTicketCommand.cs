using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.Complete
{
    public class CompleteTicketCommand : ICommand<Unit>
    {
        public UserId UserId {  get; init; }
        public List<string> Roles { get; init; }
        public TicketId TicketId { get; init; }
    }
}
