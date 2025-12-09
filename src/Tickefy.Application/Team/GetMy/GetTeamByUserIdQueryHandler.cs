using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.GetMy
{
    public class GetTeamByUserIdQueryHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IMapper mapper) : IQueryHandler<GetTeamByUserIdQuery, List<TeamResult>>
    {

        public async Task<List<TeamResult>> Handle(GetTeamByUserIdQuery query, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(query.UserId);
            if (user == null) throw new NotFoundException(nameof(user), query.UserId);

            var team = await teamRepository.GetByMemberIdAsync(query.UserId);

            return mapper.Map<List<TeamResult>>(team);
        }
    }
}
