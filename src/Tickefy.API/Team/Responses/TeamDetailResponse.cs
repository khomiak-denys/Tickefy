using Tickefy.Application.User.Common;

namespace Tickefy.API.Team.Responses
{
    public record TeamDetailResponse
    (
        Guid Id,
        string Name,
        string Description,
        string Category,
        Guid ManagerId,
        List<UserResult> Members
    );
}
