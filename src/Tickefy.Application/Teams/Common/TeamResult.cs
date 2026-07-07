using Tickefy.Application.Users.Common;
using Tickefy.Domain.Teams;

namespace Tickefy.Application.Teams.Common
{
    public record TeamResult
     (
         Guid Id,
         string Name,
         string Category,
         UserResult Manager
     )
    {
        public static TeamResult FromEntity(Team team) => new(
            team.Id.Value,
            team.Name,
            team.Category.ToString(),
            team.Manager is not null ? UserResult.FromEntity(team.Manager) : new UserResult(team.ManagerId?.Value ?? Guid.Empty, string.Empty, string.Empty)
        );
    }
}
