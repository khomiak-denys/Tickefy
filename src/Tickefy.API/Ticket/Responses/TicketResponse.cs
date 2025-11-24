namespace Tickefy.API.Ticket.Responses
{
    public record TicketResponse(
        Guid Id,
        string Title,
        Guid RequesterId,
        Guid? AssignedTeamId,
        Guid? AssignedAgentId,
        string Category,
        string Priority,
        string Status,
        DateTime Deadline
        );
}
