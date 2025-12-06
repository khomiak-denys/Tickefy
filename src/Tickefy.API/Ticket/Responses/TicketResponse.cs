using Tickefy.API.Team.Responses;
using Tickefy.API.User.Responses;

namespace Tickefy.API.Ticket.Responses
{
    /*public record TicketResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public MinimalUserResponse Requester { get; init; }
        public TeamResponse? AssignedTeamId { get; init; }
        public MinimalUserResponse? AssignedAgent { get; init; }
        public string Category { get; init; }
        public string Priority { get; init; }
        public string Status { get; init; }
        public DateTime Deadline { get; init; }
    }*/

    public record TicketResponse(
        Guid Id,
        string Title,
        MinimalUserResponse Requester,
        TeamResponse? AssignedTeamId,
        MinimalUserResponse? AssignedAgent,
        string Category,
        string Priority,
        string Status,
        DateTime Deadline
    );

}
