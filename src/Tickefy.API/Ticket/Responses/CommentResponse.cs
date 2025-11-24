namespace Tickefy.API.Ticket.Responses
{
    public record CommentResponse(
        Guid Id,
        Guid UserId,
        string Content,
        DateTime Created
        );
}
