using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Ticket
{
    public class Ticket : EntityBase<TicketId>
    {
        public string Title { get; private set; }
        public string Description { get; private set; }

        public UserId RequesterId { get; private set; }
        public Domain.User.User Requester { get; private set; } = null!;

        public TeamId? AssignedTeamId { get; private set; }
        public Domain.Team.Team? AssignedTeam { get; private set; }

        public UserId? AssignedAgentId { get; private set; }
        public Domain.User.User? AssignedAgent { get; private set; }

        public Category? Category { get; private set; }
        public Priority? Priority { get; private set; }
        public Status Status { get; private set; } = Status.Created;

        public DateTime Deadline { get; private set; }

        public List<Domain.Comment.Comment> Comments { get; private set; } = new();
        public List<Domain.Attachment.Attachment> Attachments { get; private set; } = new();

        private Ticket() { }

        public static Ticket Create(string title, string description, UserId requesterId, DateTime deadline)
        {
            var ticket = new Ticket(title, description, requesterId, deadline);
            ticket.OnCreate();
            return ticket;
        }

        public void SetCategory(Category category)
        {
            Category = category;
        }
        public void SetPriority(Priority priority)
        {
            Priority = priority;
        }

        public void AddComment(Domain.Comment.Comment comment)
        {
            Comments.Add(comment);
        }

        public void Complete()
        {
            Status = Status.Completed;
        }
        public void Revise()
        {
            Status = Status.Assigned;
        }
        public void Cancel()
        {
            Status = Status.Canceled;
        }

        private Ticket(string title, string description, UserId requesterId, DateTime deadline)
        {
            Id = new TicketId();
            Title = title;
            Description = description;
            RequesterId = requesterId;
            Deadline = deadline;
        }
    }
}
