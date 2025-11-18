using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.ActivityLog
{
    public class ActivityLog : EntityBase<ActivityLogId>
    {
        public required TicketId TicketId { get; set; }
        public required UserId UserId { get; set; }
        public required EventType EventType { get; set; }
        public required string Description { get; set; }
    }
}
