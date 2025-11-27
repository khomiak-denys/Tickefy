using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Team;

namespace Tickefy.Application.Team.GetMy
{
    public class GetTeamByUserIdQueryHandler : IQueryHandler<GetTeamByUserIdQuery, TeamDetailsResult>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetTeamByUserIdQueryHandler(
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<TeamDetailsResult> Handle(GetTeamByUserIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId);
            if (user == null) throw new NotFoundException(nameof(user), query.UserId);

            var team = await _teamRepository.GetByManagerIdAsync(query.UserId);
            if (team == null) throw new NotFoundException(nameof(team));

            return _mapper.Map<TeamDetailsResult>(team);
        }
    }
}
