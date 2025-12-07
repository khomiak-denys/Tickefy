using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.GetQueue
{
    public class GetQueueTicketsQuery : IQuery<List<TicketResult>>
    {
        public UserId UserId { get; init; }

        public GetQueueTicketsQuery(UserId userId)
        {
            UserId = userId;
        }
    }
}
