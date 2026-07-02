using Tickefy.Application.Teams.Common;
using Tickefy.Application.Users.Common;

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
        );
}
