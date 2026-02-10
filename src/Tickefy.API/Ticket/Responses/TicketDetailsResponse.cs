using Tickefy.API.Team.Responses;
using Tickefy.API.User.Responses;
using Tickefy.Domain.Common.Action;

namespace Tickefy.API.Ticket.Responses
{
    public record TicketDetailsResponse(
        Guid Id,
        string Title,
        string Description,
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
        List<ActionResponse> AvailableActions
    );
}