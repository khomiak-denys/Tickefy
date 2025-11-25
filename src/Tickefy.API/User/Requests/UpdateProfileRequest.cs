using Tickefy.Application.User.UpdateProfile;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.User.Requests
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UpdateProfileCommand ToCommand(UserId userId)
        {
            return new UpdateProfileCommand(userId, FirstName, LastName);
        }
    }
}
