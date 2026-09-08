using Tickefy.Application.Teams.Common;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.Common
{
    public record TicketDetailsResult(
        Guid Id,
        string Title,
        string? Description,
        UserResult Requester,
        TeamResult? AssignedTeam,
        UserResult? AssignedAgent,
        string Category,
        string Priority,
        string Status,
        DateTime Created,
        DateTime Deadline,
        List<CommentResult> Comments
    )
    {
        public required IEnumerable<AttachmentResult> Attachments { get; set; }
        public required IEnumerable<TicketActionResult> AvailableActions { get; set; }

        public static TicketDetailsResult FromEntity(Ticket ticket) => new(
            ticket.Id.Value,
            ticket.Title,
            ticket.Description,
            UserResult.FromEntity(ticket.Requester),
            ticket.AssignedTeam is not null ? TeamResult.FromEntity(ticket.AssignedTeam) : null,
            ticket.AssignedAgent is not null ? UserResult.FromEntity(ticket.AssignedAgent) : null,
            ticket.Category?.ToString() ?? string.Empty,
            ticket.Priority?.ToString() ?? string.Empty,
            ticket.Status.ToString(),
            ticket.Created,
            ticket.Deadline,
            ticket.Comments.Select(CommentResult.FromEntity).ToList()
        )
        {
            Attachments = Enumerable.Empty<AttachmentResult>(),
            AvailableActions = Enumerable.Empty<TicketActionResult>(),
        };
    }
}
