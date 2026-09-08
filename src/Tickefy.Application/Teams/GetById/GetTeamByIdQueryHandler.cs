using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;

namespace Tickefy.Application.Teams.GetById
{
    public class GetTeamByIdQueryHandler : IQueryHandler<GetMyTeamQuery, Result<TeamDetailsResult>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ILogger<GetTeamByIdQueryHandler> _logger;

        public GetTeamByIdQueryHandler(
            ITeamRepository teamRepository,
            ILogger<GetTeamByIdQueryHandler> logger)
        {
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task<Result<TeamDetailsResult>> Handle(GetMyTeamQuery query, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(query.TeamId, cancellationToken);
            if (team == null)
            {
                _logger.LogWarning("Team {TeamId} not found", query.TeamId.Value);
                return Result<TeamDetailsResult>.Failure(new NotFoundError(nameof(team) + " " + query.TeamId));
            }

            var result = TeamDetailsResult.FromEntity(team);

            _logger.LogInformation("Retrieved team {TeamId}", query.TeamId.Value);

            return Result<TeamDetailsResult>.Success(result);
        }
    }
}
