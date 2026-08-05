using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Teams.GetAll
{
    public class GetAllTeamsQueryHandler : IQueryHandler<GetAllTeamsQuery, Result<PaginationResult<TeamResult>>>
    {
        private readonly ITeamRepository _teamRepository;
        public GetAllTeamsQueryHandler(
            ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<Result<PaginationResult<TeamResult>>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
            var pagedData = await _teamRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

            var pagedTeams = pagedData.Items
                .Select(TeamResult.FromEntity)
                .ToList();

            var result = PaginationResult<TeamResult>.Create(pagedTeams, request.PageNumber, request.PageSize, pagedData.TotalCount);

            return Result<PaginationResult<TeamResult>>.Success(result);
        }
    }
}
