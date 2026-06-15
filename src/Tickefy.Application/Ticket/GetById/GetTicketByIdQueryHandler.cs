using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.GetById
{
    public class GetTicketByIdQueryHandler(
        ITicketRepository ticketRepository,
        IMapper mapper) : IQueryHandler<GetTicketByIdQuery, Result<TicketDetailsResult>>
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

            var result = mapper.Map<TicketDetailsResult>(ticket);

            result.AvailableActions = ticket.GetAvailableActions()
                .Where(act => act.CanExecute(ticket, query.UserId, query.Roles))
                .Select(act => new TicketActionResult(act.ToString(), act.RequireReason()));

            return Result<TicketDetailsResult>.Success(result);
        }
    }
}
