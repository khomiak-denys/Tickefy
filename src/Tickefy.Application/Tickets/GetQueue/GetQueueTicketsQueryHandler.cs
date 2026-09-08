using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Tickets;
using Tickefy.Domain.Users;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Tickets.GetQueue
{
    public class GetQueueTicketsQueryHandler : IQueryHandler<GetQueueTicketsQuery, Result<PaginationResult<TicketResult>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<GetQueueTicketsQueryHandler> _logger;

        public GetQueueTicketsQueryHandler(
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            ITicketRepository ticketRepository,
            ILogger<GetQueueTicketsQueryHandler> logger)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _ticketRepository = ticketRepository;
            _logger = logger;
        }
        public async Task<Result<PaginationResult<TicketResult>>> Handle(GetQueueTicketsQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when fetching queue tickets", query.UserId.Value);
                return Result<PaginationResult<TicketResult>>.Failure(new NotFoundError(nameof(user) + " " + query.UserId));
            }

            if (user.Role != UserRoles.Agent)
            {
                _logger.LogWarning("User {UserId} with role {Role} forbidden from accessing agent queue", query.UserId.Value, user.Role);
                return Result<PaginationResult<TicketResult>>.Failure(new ForbiddenError("Only for agents"));
            }

            if (user.TeamId is null)
            {
                _logger.LogWarning("Agent {UserId} is not assigned to a team and cannot access queue", query.UserId.Value);
                return Result<PaginationResult<TicketResult>>.Failure(new ForbiddenError("Agent should be in a team"));
            }

            var team = await _teamRepository.GetByIdAsync(user.TeamId, cancellationToken);
            if (team == null)
            {
                _logger.LogWarning("Team {TeamId} not found for agent {UserId}", user.TeamId.Value, query.UserId.Value);
                return Result<PaginationResult<TicketResult>>.Failure(new NotFoundError(nameof(team)));
            }

            var pagedData = await _ticketRepository.GetCreatedByCategoryAsync(team.Category, query.Page, query.PageSize, cancellationToken);

            var pagedTickets = pagedData.Items
                .Select(TicketResult.FromEntity)
                .ToList();

            var result = PaginationResult<TicketResult>.Create(pagedTickets, query.Page, query.PageSize, pagedData.TotalCount);

            _logger.LogInformation("Retrieved {Count} queue tickets for agent {UserId} in team {TeamId} (Total: {TotalCount})", pagedTickets.Count, query.UserId.Value, team.Id.Value, pagedData.TotalCount);

            return Result<PaginationResult<TicketResult>>.Success(result);
        }
    }
}
