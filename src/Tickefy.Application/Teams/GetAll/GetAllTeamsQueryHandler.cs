using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GetAllTeamsQueryHandler> _logger;

        public GetAllTeamsQueryHandler(
            ITeamRepository teamRepository,
            ILogger<GetAllTeamsQueryHandler> logger)
        {
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task<Result<PaginationResult<TeamResult>>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
            var pagedData = await _teamRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

            var pagedTeams = pagedData.Items
                .Select(TeamResult.FromEntity)
                .ToList();

            var result = PaginationResult<TeamResult>.Create(pagedTeams, request.Page, request.PageSize, pagedData.TotalCount);

            _logger.LogInformation("Retrieved {Count} teams (Total: {TotalCount}, Page: {Page}, PageSize: {PageSize})", pagedTeams.Count, pagedData.TotalCount, request.Page, request.PageSize);

            return Result<PaginationResult<TeamResult>>.Success(result);
        }
    }
}
