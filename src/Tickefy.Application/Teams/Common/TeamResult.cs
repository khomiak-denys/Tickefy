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
            UserResult.FromEntity(team.Manager)
        );
    }
}
