using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common;
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

            var isRequester = ticket.RequesterId == query.UserId;
            var isAdmin = query.Roles.Contains(UserRoles.Admin.ToString());
            var isAssignedAgent = ticket.AssignedAgentId?.Value == query.UserId.Value || query.Roles.Contains(UserRoles.Agent.ToString());

            if (!isRequester && !isAdmin && !isAssignedAgent) throw new ForbiddenException("Invalid role");

            var result = _mapper.Map<TicketDetailsResult>(ticket);

            return result;
        }
    }
}
