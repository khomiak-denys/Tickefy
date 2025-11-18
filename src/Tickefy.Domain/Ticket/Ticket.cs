using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Ticket
{
    public class Ticket : EntityBase<TicketId>
    {
        public required string Title { get; set; }
        public required UserId RequesterId { get; set; }
        public required TeamId AssignedTeamId { get; set; }
        public required UserId AssignedAgentId { get; set; }
        public Category Category { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; } = Status.Created;
        public DateTime Deadline { get; set; }
        public required string Description { get; set; }

        public List<Domain.Attachment.Attachment> Attachments { get; set; }
    }
}
