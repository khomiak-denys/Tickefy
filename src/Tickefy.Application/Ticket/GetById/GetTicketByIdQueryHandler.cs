using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.GetById
{
    public class GetTicketByIdQueryHandler : IQueryHandler<GetTicketByIdQuery, TicketDetailsResult>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public GetTicketByIdQueryHandler(
            ITicketRepository ticketRepository,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }
        public async Task<TicketDetailsResult> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(query.TicketId, cancellationToken);
            if (ticket == null)
            {
                throw new NotFoundException(nameof(ticket), query.TicketId);
            }

            var isAgent = query.Roles.Contains(nameof(UserRoles.Agent));
            var isRequester = ticket.RequesterId == query.UserId;
            var isAdmin = query.Roles.Contains(nameof(UserRoles.Admin));
            var isAssignedAgent = ticket.AssignedAgentId?.Value == query.UserId.Value || isAgent;

            if (!isRequester && !isAdmin && !isAssignedAgent) throw new ForbiddenException("Invalid role");

            var result = _mapper.Map<TicketDetailsResult>(ticket);

            result.AvaliableActions = ticket.GetAvailableActions()
                .Where(act => act.CanExecute(isAdmin, isRequester, isAssignedAgent, isAgent))
                .Select(act => new ActionResult(act.ToString(), act.RequireReason()));
                

            return result;
        }
    }
}
