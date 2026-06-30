using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;
using Tickefy.Domain.Users;

namespace Tickefy.Domain.Comments
{
    public class Comment : EntityBase<CommentId>
    {
        public UserId UserId { get; private set; }
        public User User { get; private set; }
        public TicketId TicketId { get; private set; }
        public Ticket Ticket { get; private set; }

        public string Content { get; init; }

        private Comment() { }

        public static Comment Create(UserId userId, TicketId ticketId, string content)
        {
            var comment = new Comment(userId, ticketId, content);
            comment.OnCreate();
            return comment;
        }

        private Comment(UserId userId, TicketId ticketId, string content)
        {
            Id = new CommentId();
            UserId = userId;
            TicketId = ticketId;
            Content = content;
        }
    }
}
