using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.ActivityLog
{
    public class ActivityLog : EntityBase<ActivityLogId>
    {
        public TicketId TicketId { get; private set; }
        public Domain.Ticket.Ticket Ticket { get; private set; }
        public UserId UserId { get; private set; }
        public Domain.User.User User { get; private set; }
        public EventType EventType { get; private set; }
        public string Description { get; private set; }


        private ActivityLog() { }

        public static ActivityLog Create(TicketId ticketId, UserId userId, EventType eventType, string description)
        {
            var activityLog = new ActivityLog(ticketId, userId, eventType, description);
            activityLog.OnCreate();
            return activityLog;
        }

        private ActivityLog(TicketId ticketId, UserId userId, EventType eventType, string description)
        {
            Id = new ActivityLogId();
            TicketId = ticketId;
            UserId = userId;
            EventType = eventType;
            Description = description;
        }
    }
}
