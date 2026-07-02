using Tickefy.Application.Users.Common;

namespace Tickefy.Application.Teams.Common
{
    public record TeamDetailsResult
    (
        Guid Id,
        string Name,
        string Description,
        string Category,
        UserResult Manager,
        List<UserResult> Members
    );
}
