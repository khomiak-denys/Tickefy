using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.GetById
{
    public class GetTicketByIdQueryHandler(
        ITicketRepository ticketRepository,
        IMapper mapper) : IQueryHandler<GetTicketByIdQuery, TicketDetailsResult>
    {
        public async Task<TicketDetailsResult> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.GetByIdAsync(query.TicketId, cancellationToken);
            if (ticket == null)
            {
                throw new NotFoundException(nameof(ticket), query.TicketId);
            }

            var isAgent = query.Roles.Contains(nameof(UserRoles.Agent));
            var isRequester = ticket.RequesterId == query.UserId;
            var isAdmin = query.Roles.Contains(nameof(UserRoles.Admin));
            var isAssignedAgent = ticket.AssignedAgentId?.Value == query.UserId.Value || isAgent;

            if (!isRequester && !isAdmin && !isAssignedAgent) throw new ForbiddenException("Invalid role");

            var result = mapper.Map<TicketDetailsResult>(ticket);

            result.AvailableActions = ticket.GetAvailableActions()
                .Where(act => act.CanExecute(ticket, isAdmin, isRequester, isAssignedAgent, isAgent))
                .Select(act => new TicketActionResult(act.ToString(), act.RequireReason()));
            
            return result;
        }
    }
}