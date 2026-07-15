using Tickefy.API.Team.Responses;
using Tickefy.API.User.Responses;
using Tickefy.Application.Tickets.Common;

namespace Tickefy.API.Ticket.Responses
{
    public record TicketDetailsResponse(
        Guid Id,
        string Title,
        string? Description,
        MinimalUserResponse Requester,
        TeamResponse? AssignedTeam,
        MinimalUserResponse? AssignedAgent,
        string Category,
        string Priority,
        string Status,
        DateTime Created,
        DateTime Deadline,
        List<CommentResponse> Comments,
        List<AttachmentResponse> Attachments,
        List<TicketActionResponse> AvailableActions
    )
    {
        public static TicketDetailsResponse FromResult(TicketDetailsResult result) => new(
            result.Id,
            result.Title,
            result.Description,
            MinimalUserResponse.FromResult(result.Requester),
            result.AssignedTeam is not null ? TeamResponse.FromResult(result.AssignedTeam) : null,
            result.AssignedAgent is not null ? MinimalUserResponse.FromResult(result.AssignedAgent) : null,
            result.Category,
            result.Priority,
            result.Status,
            result.Created,
            result.Deadline,
            result.Comments.Select(CommentResponse.FromResult).ToList(),
            result.Attachments.Select(AttachmentResponse.FromResult).ToList(),
            result.AvailableActions.Select(TicketActionResponse.FromResult).ToList()
        );
    }
}
