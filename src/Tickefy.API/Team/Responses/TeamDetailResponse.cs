using Tickefy.API.User.Responses;
using Tickefy.Application.Teams.Common;

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
    )
    {
        public static TeamDetailResponse FromResult(TeamDetailsResult result) => new(
            result.Id,
            result.Name,
            result.Description,
            result.Category,
            MinimalUserResponse.FromResult(result.Manager),
            result.Members.Select(MinimalUserResponse.FromResult).ToList()
        );
    }
}
