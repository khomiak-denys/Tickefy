namespace Tickefy.API.ActivityLog.Responses
{
    public record LogResponse(
        Guid Id,
        Guid TicketId,
        Guid UserId,
        string EventType,
        string Description,
        DateTime Created
        );
}
