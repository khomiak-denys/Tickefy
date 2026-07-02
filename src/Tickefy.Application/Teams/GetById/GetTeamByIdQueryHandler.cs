using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;

namespace Tickefy.Application.Teams.GetById
{
    public class GetTeamByIdQueryHandler : IQueryHandler<GetMyTeamQuery, Result<TeamDetailsResult>>
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

        public async Task<Result<TeamDetailsResult>> Handle(GetMyTeamQuery query, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(query.TeamId, cancellationToken);
            if (team == null) return Result<TeamDetailsResult>.Failure(new NotFoundError(nameof(team) + " " + query.TeamId));

            var result = _mapper.Map<TeamDetailsResult>(team);

            return Result<TeamDetailsResult>.Success(result);
        }
    }
}
