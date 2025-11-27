using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Team;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.GetQueue
{
    public class GetQueueTicketsQueryHandler : IQueryHandler<GetQueueTicketsQuery, List<TicketDetailsResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public GetQueueTicketsQueryHandler(
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            ITicketRepository ticketRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }
        public async Task<List<TicketDetailsResult>> Handle(GetQueueTicketsQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId);
            if (user == null) throw new NotFoundException(nameof(user), query.UserId);

            if (user.Role != UserRoles.Agent) throw new ForbiddenException("Only for agents");

            if (user.TeamId is null) throw new ForbiddenException("Agent should be in a team");
            var team = await _teamRepository.GetByIdAsync(user.TeamId);

            if (team == null) throw new NotFoundException(nameof(team));

            var tickets = await _ticketRepository.GetCreatedByCategory(team.Category);

            return _mapper.Map<List<TicketDetailsResult>>(tickets);
        }
    }
}
