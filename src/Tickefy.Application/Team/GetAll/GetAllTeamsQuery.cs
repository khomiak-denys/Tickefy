using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Team.GetAll
{
    public class GetAllTeamsQuery : IQuery<Result<List<TeamResult>>>
    {
        public GetAllTeamsQuery() { }
    }
}
