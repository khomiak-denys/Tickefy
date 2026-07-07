using Tickefy.Application.Users.Common;
using Tickefy.Domain.Teams;

namespace Tickefy.Application.Teams.Common
{
    public record TeamDetailsResult
    (
        Guid Id,
        string Name,
        string Description,
        string Category,
        UserResult Manager,
        List<UserResult> Members
    )
    {
        public static TeamDetailsResult FromEntity(Team team) => new(
            team.Id.Value,
            team.Name,
            team.Description ?? string.Empty,
            team.Category.ToString(),
            UserResult.FromEntity(team.Manager),
            team.Members.Select(UserResult.FromEntity).ToList()
        );
    }
}
