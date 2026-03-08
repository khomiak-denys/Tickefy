using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.Take
{
    public class TakeTicketCommand : ICommand<Result>
    {
        public UserId UserId { get; init; }
        public List<string> Roles { get; init; }
        public TicketId TicketId { get; init; }
    }
}
