using System.Diagnostics;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.Action;
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
            var ticket = new Ticket(title, description, Status.Created,  requesterId, deadline);
            ticket.OnCreate();
            return ticket;
        }

        public static Ticket CreateDraft(string title, string description, UserId requesterId, DateTime deadline)
        {
            var ticket = new Ticket(title, description, Status.Draft,  requesterId, deadline);
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
        
        private Ticket(string title, string description, Status status, UserId requesterId, DateTime deadline)
        {
            Id = new TicketId();
            Title = title;
            Description = description;
            Status = status;
            RequesterId = requesterId;
            Deadline = deadline;
        }

        public void Take(UserId agentId, TeamId teamId)
        {
            if (!GetAvailableActions().Contains(TicketAction.Take))
            {
                throw new ForbiddenException("Invalid action");
            }
            AssignedAgentId = agentId;
            AssignedTeamId = teamId;
            Status = Status.Assigned;
        }

        public void Start()
        {
            if (!GetAvailableActions().Contains(TicketAction.Start))
            {
                throw new ForbiddenException("Invalid action");
            }

            Status = Status.InProgress;
        }

        public void Complete()
        {
            if (!GetAvailableActions().Contains(TicketAction.Complete))
            {
                throw new ForbiddenException("Invalid action");
                
            }
            Status = Status.Completed;
        }

        public void Reopen()
        {
            if (!GetAvailableActions().Contains(TicketAction.Reopen))
            {
                throw new ForbiddenException("Invalid action");
            }
            Status = Status.Reopened;
        }

        public void Fail()
        {
            if (!GetAvailableActions().Contains(TicketAction.Fail))
            {
                throw new ForbiddenException("Invalid action");
            }
            Status = Status.Failed;
        }

        public void Cancel()
        {
            if (!GetAvailableActions().Contains(TicketAction.Cancel))
            {
                throw new ForbiddenException("Invalid action");
            }
            Status = Status.Canceled;
        }

        public IEnumerable<TicketAction> GetAvailableActions()
        {
            return Status switch
            {
                Status.Draft => [TicketAction.Publish],
                Status.Created => [TicketAction.Take, TicketAction.Cancel],
                Status.Assigned => [TicketAction.Cancel, TicketAction.Start],
                Status.InProgress => [TicketAction.Complete, TicketAction.Cancel, TicketAction.Fail],
                Status.Completed => [TicketAction.Accept, TicketAction.Reopen],
                Status.Reopened => [TicketAction.Start],
                _ => []
            };
        }
    }
}
