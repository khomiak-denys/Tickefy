using Tickefy.API.User.Responses;

namespace Tickefy.API.Ticket.Responses
{
    public record CommentResponse(
        Guid Id,
        MinimalUserResponse User,
        string Content,
        DateTime Created
        );
}
