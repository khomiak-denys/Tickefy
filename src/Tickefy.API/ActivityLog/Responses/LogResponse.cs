using Tickefy.API.User.Responses;
using Tickefy.Application.ActivityLogs.Common;

namespace Tickefy.API.ActivityLog.Responses
{
    public record LogResponse(
        Guid Id,
        Guid TicketId,
        MinimalUserResponse User,
        string EventType,
        string Description,
        DateTime Created
        )
    {
        public static LogResponse FromResult(LogResult result) => new(
            result.Id,
            result.TicketId,
            MinimalUserResponse.FromResult(result.User),
            result.EventType,
            result.Description,
            result.Created
        );
    }
}
