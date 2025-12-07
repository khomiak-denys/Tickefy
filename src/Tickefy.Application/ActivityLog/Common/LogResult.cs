using Tickefy.Application.User.Common;

namespace Tickefy.Application.ActivityLog.Common
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
