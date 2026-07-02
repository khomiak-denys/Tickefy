using Tickefy.Application.Users.Common;

namespace Tickefy.Application.ActivityLogs.Common
{
    public record LogResult(
        Guid Id,
        Guid TicketId,
        UserResult User,
        string EventType,
        string Description,
        DateTime Created
        );
}
