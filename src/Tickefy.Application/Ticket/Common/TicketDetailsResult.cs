using Tickefy.Application.Team.Common;
using Tickefy.Application.User.Common;

namespace Tickefy.Application.Ticket.Common
{
    public record TicketDetailsResult(
        Guid Id,
        string Title, 
        string Description,
        UserResult Requester,
        TeamResult? AssignedTeam,
        UserResult? AssignedAgent, 
        string Category,
        string Priority,
        string Status,
        DateTime Deadline,
        List<CommentResult> Comments,
        List<AttachmentResult> Attachments
        );
}
