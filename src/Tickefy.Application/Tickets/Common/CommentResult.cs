using Tickefy.Application.Users.Common;
using Tickefy.Domain.Comments;

namespace Tickefy.Application.Tickets.Common
{
    public record CommentResult(
         Guid Id,
         UserResult User,
         string Content,
         DateTime Created
    )
    {
        public static CommentResult FromEntity(Comment comment) => new(
            comment.Id.Value,
            UserResult.FromEntity(comment.User),
            comment.Content,
            comment.Created
        );
    }
}
