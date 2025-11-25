using Tickefy.Domain.Common.UserRole;

namespace Tickefy.API.User.Responses
{
    public record UserResponse(
        Guid id,
        string FirstName,
        string LastName,
        string Login,
        string Role,
        Guid? TeamId,
        DateTime Created
        );
}
