using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.Common
{
    public record UserDetailsResult(
        Guid Id,
        string FirstName,
        string LastName,
        string Login,
        string Role,
        TeamResult? Team,
        DateTime Created
        )
    {
        public static UserDetailsResult FromEntity(User user) => new(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Login,
            user.Role.ToString(),
            user.Team is not null ? TeamResult.FromEntity(user.Team) : null,
            user.Created
        );
    }
}
