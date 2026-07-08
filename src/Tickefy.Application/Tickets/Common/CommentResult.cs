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
            comment.User is not null ? UserResult.FromEntity(comment.User) : new UserResult(comment.UserId.Value, string.Empty, string.Empty),
            comment.Content,
            comment.Created
        );
    }
}
