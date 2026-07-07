using Tickefy.API.Team.Responses;
using Tickefy.API.User.Responses;
using Tickefy.Application.Tickets.Common;

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
    )
    {
        public static TicketResponse FromResult(TicketResult result) => new(
            result.Id,
            result.Title,
            MinimalUserResponse.FromResult(result.Requester),
            result.AssignedTeam is not null ? TeamResponse.FromResult(result.AssignedTeam) : null,
            result.AssignedAgent is not null ? MinimalUserResponse.FromResult(result.AssignedAgent) : null,
            result.Category,
            result.Priority,
            result.Status,
            result.Deadline
        );
    }
}
