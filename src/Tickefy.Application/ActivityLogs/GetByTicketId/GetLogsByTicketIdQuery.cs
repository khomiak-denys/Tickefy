using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.Primitives;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.ActivityLogs.GetByTicketId
{
    public class GetLogsByTicketIdQuery : IQuery<PaginationResult<LogResult>>
    {
        public TicketId TicketId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public GetLogsByTicketIdQuery(TicketId ticketId)
        {
            TicketId = ticketId;
        }
    }
}
