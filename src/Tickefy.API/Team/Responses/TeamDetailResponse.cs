using Tickefy.API.User.Responses;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Team.Responses
{
    public record TeamDetailResponse
    (
        Guid Id,
        string Name,
        string Description,
        string Category,
        MinimalUserResponse Manager,
        List<MinimalUserResponse> Members
    );
}
