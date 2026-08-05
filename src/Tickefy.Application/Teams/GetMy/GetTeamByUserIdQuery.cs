using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Teams.GetMy
{
    public class GetTeamByUserIdQuery : IQuery<Result<PaginationResult<TeamResult>>>
    {
        public UserId UserId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public GetTeamByUserIdQuery(UserId userId)
        {
            UserId = userId;
        }
    }
}
