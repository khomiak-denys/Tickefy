using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.ActivityLogs.GetByTicketId
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
