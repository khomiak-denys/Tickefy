using Tickefy.API.Team.Responses;

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
        );
}
