using Tickefy.Application.Team.Common;
using Tickefy.Application.User.Common;

namespace Tickefy.Application.Ticket.Common
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
