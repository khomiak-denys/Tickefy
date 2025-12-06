using Tickefy.API.User.Responses;

namespace Tickefy.API.ActivityLog.Responses
{
    public record LogResponse(
        Guid Id,
        Guid TicketId,
        UserResponse UserId,
        string EventType,
        string Description,
        DateTime Created
        );
}
