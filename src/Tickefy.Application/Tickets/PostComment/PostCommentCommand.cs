using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.PostComment
{
    public class PostCommentCommand : ICommand<Result>
    {
        public required UserId UserId { get; init; }
        public required TicketId TicketId { get; init; }
        public required string Content { get; init; }
    }
}
