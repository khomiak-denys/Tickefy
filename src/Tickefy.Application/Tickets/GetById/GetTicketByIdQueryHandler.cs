using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Tickets.Common;
using Tickefy.Application.Tickets.Common.Helpers;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.GetById
{
    public class GetTicketByIdQueryHandler(
        ITicketRepository ticketRepository,
        IObjectStorageService objectStorage) : IQueryHandler<GetTicketByIdQuery, Result<TicketDetailsResult>>
    {
        public async Task<Result<TicketDetailsResult>> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.GetByIdAsync(query.TicketId, cancellationToken);
            if (ticket == null)
            {
                return Result<TicketDetailsResult>.Failure(new NotFoundError(nameof(ticket) + " " + query.TicketId));
            }

            var isRequester = ticket.RequesterId == query.UserId;
            var isAdmin = query.Roles.Contains(nameof(UserRoles.Admin));
            var isAssignedAgent = ticket.AssignedAgentId?.Value == query.UserId.Value && query.Roles.Contains(nameof(UserRoles.Agent));

            if (!isRequester && !isAdmin && !isAssignedAgent) return Result<TicketDetailsResult>.Failure(new ForbiddenError("Access denied"));

            var result = TicketDetailsResult.FromEntity(ticket);

            var attachmentResults = new List<AttachmentResult>();

            foreach (var attachment in ticket.Attachments)
            {
                var preSignedUrl = await objectStorage.GetFileUrlAsync(attachment.FilePath);

                var attachmentResult = new AttachmentResult(preSignedUrl, attachment.FileName, attachment.ContentType.ToString(), attachment.SizeBytes);
                attachmentResults.Add(attachmentResult);
            }

            result.Attachments = attachmentResults;

            result.AvailableActions = ticket.GetAvailableActions()
                .Where(act => act.CanExecute(ticket, query.UserId, query.Roles))
                .Select(act => new TicketActionResult(act.ToString(), act.RequireReason()));

            return Result<TicketDetailsResult>.Success(result);
        }
    }
}
