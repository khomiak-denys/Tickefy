using Tickefy.API.User.Responses;

namespace Tickefy.API.Team.Responses
{
    public record TeamResponse
    (
        Guid Id,
        string Name,
        string Category,
        MinimalUserResponse Manager
    );
}
