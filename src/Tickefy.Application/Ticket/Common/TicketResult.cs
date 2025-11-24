namespace Tickefy.Application.Ticket.Common
{
    public record TicketResult(
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
