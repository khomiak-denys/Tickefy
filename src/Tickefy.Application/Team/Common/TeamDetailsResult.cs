using Tickefy.Application.User.Common;

namespace Tickefy.Application.Team.Common
{
    public record TeamDetailsResult
    (
        Guid Id,
        string Name,
        string Description,
        string Category,
        Guid ManagerId,
        List<UserResult> Members
    );
}
