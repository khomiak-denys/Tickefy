using Tickefy.API.User.Responses;
using Tickefy.Application.Ticket.Common;

namespace Tickefy.API.Ticket.Responses
{
    public record TicketDetailsResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public MinimalUserResponse Requester { get; init; }
        public Guid? AssignedTeamId { get; init; }
        public MinimalUserResponse? AssignedAgent {  get; init; }
        public string Category {  get; init; }
        public string Priority { get; init; }
        public string Status { get; init; }
        public DateTime Deadline { get; init; }
        public List<CommentResponse> Comments { get; init; }
        public List<AttachmentResponse> Attachments { get; init; }
    }
    
}
