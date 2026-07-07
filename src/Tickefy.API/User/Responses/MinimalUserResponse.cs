using Tickefy.Application.Users.Common;

namespace Tickefy.API.User.Responses
{
    public record MinimalUserResponse(
        Guid Id,
        string FirstName,
        string LastName
        )
    {
        public static MinimalUserResponse FromResult(UserResult result) => new(
            result.Id,
            result.FirstName,
            result.LastName
        );
    }
}
