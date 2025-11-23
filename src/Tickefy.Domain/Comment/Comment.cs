using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Comment
{
    public class Comment : EntityBase<CommentId>
    {
        public UserId UserId { get; private set; }  
        public Domain.User.User User { get; private set; }
        public TicketId TicketId { get; private set; }
        public Domain.Ticket.Ticket Ticket { get; private set; }

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
            UserId = userId;
            TicketId = ticketId;
            Content = content;
        }
    }
}
