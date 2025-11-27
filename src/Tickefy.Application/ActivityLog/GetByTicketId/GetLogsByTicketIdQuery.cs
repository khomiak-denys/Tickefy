using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLog.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.ActivityLog.GetByTicketId
{
    public class GetLogsByTicketIdQuery : IQuery<List<LogResult>>
    {
        public TicketId TicketId { get; init; }
        public GetLogsByTicketIdQuery(TicketId ticketId)
        {
            TicketId = ticketId;
        }
    }
}
