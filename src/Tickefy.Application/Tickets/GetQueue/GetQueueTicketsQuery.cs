using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.GetQueue
{
    public class GetQueueTicketsQuery : IQuery<Result<List<TicketResult>>>
    {
        public UserId UserId { get; init; }

        public GetQueueTicketsQuery(UserId userId)
        {
            UserId = userId;
        }
    }
}
