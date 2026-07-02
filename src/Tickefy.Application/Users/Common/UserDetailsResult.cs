using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

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
        );
}
