using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Teams.GetAll
{
    public class GetAllTeamsQuery : IQuery<Result<PaginationResult<TeamResult>>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public GetAllTeamsQuery() { }
    }
}
