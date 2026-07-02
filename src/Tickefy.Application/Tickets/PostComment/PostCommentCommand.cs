using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.PostComment
{
    public class PostCommentCommand : ICommand<Result>
    {
        public UserId UserId { get; init; }
        public TicketId TicketId { get; init; }
        public string Content { get; init; }
    }
}
