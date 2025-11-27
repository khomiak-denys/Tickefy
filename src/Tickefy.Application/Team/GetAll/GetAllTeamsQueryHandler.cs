using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Team;

namespace Tickefy.Application.Team.GetAll
{
    public class GetAllTeamsQueryHandler : IQueryHandler<GetAllTeamsQuery, List<TeamResult>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public GetAllTeamsQueryHandler(
            ITeamRepository teamRepository,
            IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<List<TeamResult>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAll();

            var result = _mapper.Map<List<TeamResult>>(teams);
            return result;
        }
    }
}
