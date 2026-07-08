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
        public GetTeamByIdQueryHandler(
            ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<Result<TeamDetailsResult>> Handle(GetMyTeamQuery query, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(query.TeamId, cancellationToken);
            if (team == null) return Result<TeamDetailsResult>.Failure(new NotFoundError(nameof(team) + " " + query.TeamId));

            var result = TeamDetailsResult.FromEntity(team);

            return Result<TeamDetailsResult>.Success(result);
        }
    }
}
