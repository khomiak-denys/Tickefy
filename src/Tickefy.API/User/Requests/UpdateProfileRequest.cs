using Tickefy.Application.Users.UpdateProfile;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.User.Requests
{
    public class UpdateProfileRequest
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }

        public UpdateProfileCommand ToCommand(UserId userId)
        {
            return new UpdateProfileCommand(userId, FirstName, LastName);
        }
    }
}
