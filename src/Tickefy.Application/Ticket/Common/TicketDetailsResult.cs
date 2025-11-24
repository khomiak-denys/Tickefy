namespace Tickefy.Application.Ticket.Common
{
    public record TicketDetailsResult(
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
        List<CommentResult> Comments,
        List<AttachmentResult> Attachments
        );
}
