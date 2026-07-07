using Tickefy.API.Team.Responses;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.User.Responses
{
    public record UserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Login,
        string Role,
        TeamResponse? Team,
        DateTime Created
        )
    {
        public static UserResponse FromResult(UserDetailsResult result) => new(
            result.Id,
            result.FirstName,
            result.LastName,
            result.Login,
            result.Role,
            result.Team is not null ? TeamResponse.FromResult(result.Team) : null,
            result.Created
        );
    }
}
