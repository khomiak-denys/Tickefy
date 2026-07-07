using Tickefy.Application.Teams.Common;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.Common
{
    public record TicketResult(
        Guid Id,
        string Title,
        UserResult Requester,
        TeamResult? AssignedTeam,
        UserResult? AssignedAgent,
        string Category,
        string Priority,
        string Status,
        DateTime Deadline
        )
    {
        public static TicketResult FromEntity(Ticket ticket) => new(
            ticket.Id.Value,
            ticket.Title,
            ticket.Requester is not null ? UserResult.FromEntity(ticket.Requester) : new UserResult(ticket.RequesterId.Value, string.Empty, string.Empty),
            ticket.AssignedTeam is not null ? TeamResult.FromEntity(ticket.AssignedTeam) : null,
            ticket.AssignedAgent is not null ? UserResult.FromEntity(ticket.AssignedAgent) : null,
            ticket.Category?.ToString() ?? string.Empty,
            ticket.Priority?.ToString() ?? string.Empty,
            ticket.Status.ToString(),
            ticket.Deadline
        );
    }
}
