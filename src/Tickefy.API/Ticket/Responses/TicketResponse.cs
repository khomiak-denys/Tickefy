using Tickefy.API.Team.Responses;
using Tickefy.API.User.Responses;

namespace Tickefy.API.Ticket.Responses
{
    public record TicketResponse(
        Guid Id,
        string Title,
        MinimalUserResponse Requester,
        TeamResponse? AssignedTeam,
        MinimalUserResponse? AssignedAgent,
        string Category,
        string Priority,
        string Status,
        DateTime Deadline
    );
}
