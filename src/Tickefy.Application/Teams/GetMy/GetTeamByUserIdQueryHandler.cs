using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Teams.GetMy
{
    public class GetTeamByUserIdQueryHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository) : IQueryHandler<GetTeamByUserIdQuery, Result<List<TeamResult>>>
    {

        public async Task<Result<List<TeamResult>>> Handle(GetTeamByUserIdQuery query, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null) return Result<List<TeamResult>>.Failure(new NotFoundError(nameof(user) + " " + query.UserId));

            var teams = await teamRepository.GetByMemberIdAsync(query.UserId, cancellationToken);

            return Result<List<TeamResult>>.Success(teams.Select(TeamResult.FromEntity).ToList());
        }
    }
}
