using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Tickets.GetQueue
{
    public class GetQueueTicketsQuery : IQuery<Result<PaginationResult<TicketResult>>>
    {
        public UserId UserId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public GetQueueTicketsQuery(UserId userId)
        {
            UserId = userId;
        }
    }
}
