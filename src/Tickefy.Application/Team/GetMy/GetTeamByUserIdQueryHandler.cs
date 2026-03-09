using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.GetMy
{
    public class GetTeamByUserIdQueryHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IMapper mapper) : IQueryHandler<GetTeamByUserIdQuery, Result<List<TeamResult>>>
    {

        public async Task<Result<List<TeamResult>>> Handle(GetTeamByUserIdQuery query, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(query.UserId);
            if (user == null) return Result<List<TeamResult>>.Failure(new NotFoundError(nameof(user) + " " + query.UserId));

            var teams = await teamRepository.GetByMemberIdAsync(query.UserId);

            return Result<List<TeamResult>>.Success(mapper.Map<List<TeamResult>>(teams));
        }
    }
}
