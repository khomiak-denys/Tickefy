using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.Revise
{
    public class ReopenTicketCommand : ICommand<Result>
    {
        public UserId UserId { get; init; }
        public List<string> Roles { get; init; }
        public TicketId TicketId { get; init; }
        public string Reason { get; init; }
    }
}
