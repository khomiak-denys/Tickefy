namespace Tickefy.Application.Ticket.Common
{
    public record CommentResult(
         Guid Id ,
         Guid UserId, 
         string Content, 
         DateTime Created 
    );
}
