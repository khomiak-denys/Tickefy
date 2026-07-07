using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;

namespace Tickefy.Application.Teams.GetAll
{
    public class GetAllTeamsQueryHandler : IQueryHandler<GetAllTeamsQuery, Result<List<TeamResult>>>
    {
        private readonly ITeamRepository _teamRepository;
        public GetAllTeamsQueryHandler(
            ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<Result<List<TeamResult>>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAllAsync(cancellationToken);

            var result = teams.Select(TeamResult.FromEntity).ToList();
            return Result<List<TeamResult>>.Success(result);
        }
    }
}
