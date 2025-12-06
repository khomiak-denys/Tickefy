using Tickefy.Application.User.Common;

namespace Tickefy.Application.Ticket.Common
{
    public record CommentResult(
         Guid Id ,
         UserResult User, 
         string Content, 
         DateTime Created 
    );
}
