using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Team.GetById
{
    public class GetMyTeamQuery : IQuery<TeamDetailsResult>
    {
        public TeamId TeamId { get; init; }

        public GetMyTeamQuery(TeamId teamId)
        {
            TeamId = teamId;
        }
    }
}
