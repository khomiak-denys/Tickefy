using Tickefy.API.User.Responses;
using Tickefy.Application.Tickets.Common;

namespace Tickefy.API.Ticket.Responses
{
    public record CommentResponse(
        Guid Id,
        MinimalUserResponse User,
        string Content,
        DateTime Created
        )
    {
        public static CommentResponse FromResult(CommentResult result) => new(
            result.Id,
            MinimalUserResponse.FromResult(result.User),
            result.Content,
            result.Created
        );
    }
}
