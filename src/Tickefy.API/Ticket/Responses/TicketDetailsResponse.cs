using Tickefy.Application.Ticket.Common;

namespace Tickefy.API.Ticket.Responses
{
    public record TicketDetailsResponse(
        Guid Id,
        string Title,
        string Description,
        Guid RequesterId,
        Guid? AssignedTeamId,
        Guid? AssignedAgentId,
        string Category,
        string Priority,
        string Status,
        DateTime Deadline,
        List<CommentResponse> Comments,
        List<AttachmentResponse> Attachments
        );
    
}
