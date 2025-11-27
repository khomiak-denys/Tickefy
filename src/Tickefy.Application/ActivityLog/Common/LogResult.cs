namespace Tickefy.Application.ActivityLog.Common
{
    public record LogResult(
        Guid Id,
        Guid TicketId,
        Guid UserId,
        string EventType,
        string Description,
        DateTime Created
        );
}
