using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.PostComment
{
    public class PostCommentCommandHandler : ICommandHandler<PostCommentCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly ILogger<PostCommentCommandHandler> _logger;

        public PostCommentCommandHandler(
            IUnitOfWork uow,
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            ILogger<PostCommentCommandHandler> logger)
        {
            _uow = uow;
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(PostCommentCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found when posting comment", command.TicketId.Value);
                return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
            }

            if (ticket.RequesterId.Value != command.UserId.Value && ticket.AssignedAgentId?.Value != command.UserId.Value)
            {
                _logger.LogWarning("User {UserId} is not authorized to comment on ticket {TicketId}", command.UserId.Value, command.TicketId.Value);
                return Result.Failure(new InvalidArgumentError(nameof(command.UserId.Value)));
            }

            var comment = Domain.Comments.Comment.Create(command.UserId, command.TicketId, command.Content);

            ticket.AddComment(comment);

            var log = Domain.ActivityLogs.ActivityLog.Create(ticket.Id, command.UserId, EventType.CommentAdded, "User added comment");
            _logRepository.Add(log);

            await _uow.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Comment posted on ticket {TicketId} by user {UserId}", command.TicketId.Value, command.UserId.Value);

            return Result.Success();
        }
    }
}
