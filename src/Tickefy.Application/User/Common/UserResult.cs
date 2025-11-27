using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.Common
{
    public record UserResult(
        Guid id,
        string FirstName,
        string LastName,
        string Login,
        string Role,
        Guid? TeamId,
        DateTime Created
        );
}
