using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.PostComment
{
    public class PostCommentCommand : ICommand
    {
        public UserId UserId { get; init; }
        public TicketId TicketId { get; init; }
        public string Content { get; init; }
    }
}
