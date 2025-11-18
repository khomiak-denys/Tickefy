using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Comment
{
    public class Comment : EntityBase<CommentId>
    {
        public required UserId UserId { get; set; }  
        public required TicketId TicketId { get; set; }
        public string Content { get; set; }
    }
}
