using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Tickets.GetMy
{
    public class GetMyTicketsQuery : IQuery<Result<PaginationResult<TicketResult>>>
    {
        public UserId UserId { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public GetMyTicketsQuery(UserId id)
        {
            UserId = id;
        }
    }
}
