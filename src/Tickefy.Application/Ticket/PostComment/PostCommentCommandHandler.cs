using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.PostComment
{
    public class PostCommentCommandHandler : ICommandHandler<PostCommentCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;

        public PostCommentCommandHandler(
            IUnitOfWork uow,
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository)
        {
            _uow = uow;
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
        }
        public async Task Handle(PostCommentCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null)
            {
                throw new NotFoundException(nameof(ticket), command.TicketId);
            }

            if (ticket.RequesterId.Value != command.UserId.Value && ticket.AssignedAgentId?.Value != command.UserId.Value)
            {
                throw new InvalidArgumentException(nameof(command.UserId.Value));
            }

            var comment = Domain.Comment.Comment.Create(command.UserId, command.TicketId, command.Content);

            ticket.AddComment(comment);

            var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.CommentAdded, "User added comment");
            _logRepository.Add(log);

            await _uow.SaveChangesAsync();
        }
    }
}
