using Tickefy.Application.Team.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.Common
{
    public record UserDetailsResult(
        Guid id,
        string FirstName,
        string LastName,
        string Login,
        string Role,
        TeamResult? Team,
        DateTime Created
        );
}
