using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;
using Tickefy.Domain.Users;

namespace Tickefy.Domain.Comments
{
    public class Comment : EntityBase<CommentId>
    {
        public UserId UserId { get; private init; } = null!;
        public User User { get; private set; } = null!;
        public TicketId TicketId { get;  init; } = null!;
        public Ticket Ticket { get; private set; } = null!;

        public string Content { get; init; } = null!;

        private Comment() { }

        public static Comment Create(UserId userId, TicketId ticketId, string content)
        {
            var comment = new Comment(userId, ticketId, content);
            comment.OnCreate();
            return comment;
        }

        private Comment(UserId userId, TicketId ticketId, string content)
        {
            UserId = userId;
            TicketId = ticketId;
            Content = content;
        }
    }
}
