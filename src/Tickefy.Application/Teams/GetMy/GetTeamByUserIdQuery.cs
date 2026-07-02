using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Teams.GetMy
{
    public class GetTeamByUserIdQuery : IQuery<Result<List<TeamResult>>>
    {
        public UserId UserId { get; init; }
        public GetTeamByUserIdQuery(UserId userId)
        {
            UserId = userId;
        }
    }
}
