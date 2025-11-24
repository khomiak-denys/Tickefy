using Tickefy.Application.Ticket.PostComment;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Ticket.Requests
{
    public class PostCommentRequest
    {
        public string Content { get; init; }

        public PostCommentCommand ToCommand(UserId userId, TicketId ticketId)
        {
            return new PostCommentCommand
            {
                UserId = userId,
                TicketId = ticketId,
                Content = Content
            };
        }
    }
}
