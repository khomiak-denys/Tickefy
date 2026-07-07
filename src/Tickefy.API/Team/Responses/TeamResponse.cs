using Tickefy.API.User.Responses;
using Tickefy.Application.Teams.Common;

namespace Tickefy.API.Team.Responses
{
    public record TeamResponse
    (
        Guid Id,
        string Name,
        string Category,
        MinimalUserResponse Manager
    )
    {
        public static TeamResponse FromResult(TeamResult result) => new(
            result.Id,
            result.Name,
            result.Category,
            MinimalUserResponse.FromResult(result.Manager)
        );
    }
}
