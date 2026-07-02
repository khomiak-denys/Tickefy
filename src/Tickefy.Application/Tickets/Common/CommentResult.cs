using Tickefy.Application.Users.Common;

namespace Tickefy.Application.Tickets.Common
{
    public record CommentResult(
         Guid Id,
         UserResult User,
         string Content,
         DateTime Created
    );
}
