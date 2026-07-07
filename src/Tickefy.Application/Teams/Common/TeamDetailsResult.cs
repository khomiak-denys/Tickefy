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
            team.Manager is not null ? UserResult.FromEntity(team.Manager) : new UserResult(team.ManagerId?.Value ?? Guid.Empty, string.Empty, string.Empty),
            team.Members.Select(UserResult.FromEntity).ToList()
        );
    }
}
