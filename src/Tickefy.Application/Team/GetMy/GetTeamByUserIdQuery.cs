using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Team.GetMy
{
    public class GetTeamByUserIdQuery : IQuery<List<TeamResult>>
    {
        public UserId UserId { get; init; }
        public GetTeamByUserIdQuery(UserId userId) 
        {
            UserId = userId;
        }
    }
}
