using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Teams.GetMy
{
    public class GetTeamByUserIdQueryHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        ILogger<GetTeamByUserIdQueryHandler> logger) : IQueryHandler<GetTeamByUserIdQuery, Result<PaginationResult<TeamResult>>>
    {
        public async Task<Result<PaginationResult<TeamResult>>> Handle(GetTeamByUserIdQuery query, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null)
            {
                logger.LogWarning("User {UserId} not found when fetching member teams", query.UserId.Value);
                return Result<PaginationResult<TeamResult>>.Failure(new NotFoundError(nameof(user) + " " + query.UserId));
            }

            var pagedData = await teamRepository.GetByMemberIdAsync(query.UserId, query.Page, query.PageSize, cancellationToken);

            var pagedTeams = pagedData.Items
                .Select(TeamResult.FromEntity)
                .ToList();

            var result = PaginationResult<TeamResult>.Create(pagedTeams, query.Page, query.PageSize, pagedData.TotalCount);

            logger.LogInformation("Retrieved {Count} teams for user {UserId} (Total: {TotalCount})", pagedTeams.Count, query.UserId.Value, pagedData.TotalCount);

            return Result<PaginationResult<TeamResult>>.Success(result);
        }
    }
}
