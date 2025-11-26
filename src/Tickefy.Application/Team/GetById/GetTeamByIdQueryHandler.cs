using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Team;

namespace Tickefy.Application.Team.GetById
{
    public class GetTeamByIdQueryHandler : IQueryHandler<GetTeamByIdQuery, TeamDetailsResult>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public GetTeamByIdQueryHandler(
            ITeamRepository teamRepository,
            IMapper mapper) 
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<TeamDetailsResult> Handle(GetTeamByIdQuery query, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(query.TeamId);
            if (team == null) throw new NotFoundException(nameof(team), query.TeamId);

            var result = _mapper.Map<TeamDetailsResult>(team);

            return result;
        }
    }
}
