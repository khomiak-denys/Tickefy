using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Results;
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
            var ticket = new Ticket(title, description, Status.Created, requesterId, deadline);
            ticket.OnCreate();
            return ticket;
        }

        public static Ticket CreateDraft(string title, string description, UserId requesterId, DateTime deadline)
        {
            var ticket = new Ticket(title, description, Status.Draft, requesterId, deadline);
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
        
        

        public Result Publish(string title, string description, DateTime deadline)
        {
            if (!GetAvailableActions().Contains(TicketAction.Publish))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
            }
            Title = title;
            Description = description;
            Deadline = deadline;
            Status = Status.Created;
            return Result.Success();
        }
        
        public Result Take(UserId agentId, TeamId teamId)
        {
            if (!GetAvailableActions().Contains(TicketAction.Take))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
            }
            AssignedAgentId = agentId;
            AssignedTeamId = teamId;
            Status = Status.Assigned;
            return Result.Success();
        }

        public Result StartWork()
        {
            if (!GetAvailableActions().Contains(TicketAction.StartWork))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
            }

            Status = Status.InProgress;
            return Result.Success();
        }

        public Result Complete()
        {
            if (!GetAvailableActions().Contains(TicketAction.Complete))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
                
            }
            Status = Status.Completed;
            return  Result.Success();
        }

        public Result Reopen()
        {
            if (!GetAvailableActions().Contains(TicketAction.Reopen))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
            }
            Status = Status.Reopened;
            return Result.Success();
        }

        public Result Accept()
        {
            if (!GetAvailableActions().Contains(TicketAction.Accept))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
            }
            Status = Status.Accepted;
            return Result.Success();
        }

        public Result Fail()
        {
            if (!GetAvailableActions().Contains(TicketAction.Fail))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
            }
            Status = Status.Failed;
            return Result.Success();
        }

        public Result Cancel()
        {
            if (!GetAvailableActions().Contains(TicketAction.Cancel))
            {
                return Result.Failure(new ForbiddenError("Invalid action"));
            }
            Status = Status.Canceled;
            return Result.Success();
        }

        public IEnumerable<TicketAction> GetAvailableActions()
        {
            return Status switch
            {
                Status.Draft => [TicketAction.Publish],
                Status.Created => [TicketAction.Take, TicketAction.Cancel],
                Status.Assigned => [TicketAction.Cancel, TicketAction.StartWork],
                Status.InProgress => [TicketAction.Complete, TicketAction.Cancel, TicketAction.Fail],
                Status.Completed => [TicketAction.Accept, TicketAction.Reopen],
                Status.Reopened => [TicketAction.StartWork],
                _ => []
            };
        }
    }
}
