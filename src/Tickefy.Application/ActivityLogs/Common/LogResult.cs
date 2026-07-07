using Tickefy.Application.Users.Common;
using Tickefy.Domain.ActivityLogs;

namespace Tickefy.Application.ActivityLogs.Common
{
    public record LogResult(
        Guid Id,
        Guid TicketId,
        UserResult User,
        string EventType,
        string Description,
        DateTime Created
        )
    {
        public static LogResult FromEntity(ActivityLog log) => new(
            log.Id.Value,
            log.TicketId.Value,
            log.User is not null ? UserResult.FromEntity(log.User) : new UserResult(log.UserId.Value, string.Empty, string.Empty),
            log.EventType.ToString(),
            log.Description,
            log.Created
        );
    }
}
