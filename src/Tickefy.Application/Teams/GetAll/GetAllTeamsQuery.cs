using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Teams.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Teams.GetAll
{
    public class GetAllTeamsQuery : IQuery<Result<List<TeamResult>>>
    {
        public GetAllTeamsQuery() { }
    }
}
