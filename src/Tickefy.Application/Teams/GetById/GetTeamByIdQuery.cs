using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Teams.GetById
{
    public class GetMyTeamQuery : IQuery<Result<TeamDetailsResult>>
    {
        public TeamId TeamId { get; init; }

        public GetMyTeamQuery(TeamId teamId)
        {
            TeamId = teamId;
        }
    }
}
