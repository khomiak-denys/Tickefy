using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Tickets;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Tickets.GetQueue
{
    public class GetQueueTicketsQueryHandler : IQueryHandler<GetQueueTicketsQuery, Result<List<TicketResult>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITicketRepository _ticketRepository;

        public GetQueueTicketsQueryHandler(
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            ITicketRepository ticketRepository)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _ticketRepository = ticketRepository;
        }
        public async Task<Result<List<TicketResult>>> Handle(GetQueueTicketsQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null) return Result<List<TicketResult>>.Failure(new NotFoundError(nameof(user) + " " + query.UserId));

            if (user.Role != UserRoles.Agent) return Result<List<TicketResult>>.Failure(new ForbiddenError("Only for agents"));

            if (user.TeamId is null) return Result<List<TicketResult>>.Failure(new ForbiddenError("Agent should be in a team"));
            var team = await _teamRepository.GetByIdAsync(user.TeamId, cancellationToken);

            if (team == null) return Result<List<TicketResult>>.Failure(new NotFoundError(nameof(team)));

            var tickets = await _ticketRepository.GetCreatedByCategoryAsync(team.Category, cancellationToken);

            return Result<List<TicketResult>>.Success(tickets.Select(TicketResult.FromEntity).ToList());
        }
    }
}
