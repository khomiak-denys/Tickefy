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
        IUserRepository userRepository) : IQueryHandler<GetTeamByUserIdQuery, Result<PaginationResult<TeamResult>>>
    {
        public async Task<Result<PaginationResult<TeamResult>>> Handle(GetTeamByUserIdQuery query, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null) return Result<PaginationResult<TeamResult>>.Failure(new NotFoundError(nameof(user) + " " + query.UserId));

            var pagedData = await teamRepository.GetByMemberIdAsync(query.UserId, query.PageNumber, query.PageSize, cancellationToken);

            var pagedTeams = pagedData.Items
                .Select(TeamResult.FromEntity)
                .ToList();

            var result = PaginationResult<TeamResult>.Create(pagedTeams, query.PageNumber, query.PageSize, pagedData.TotalCount);

            return Result<PaginationResult<TeamResult>>.Success(result);
        }
    }
}
