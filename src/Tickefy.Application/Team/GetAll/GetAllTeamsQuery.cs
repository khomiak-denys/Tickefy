using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Team.Common;

namespace Tickefy.Application.Team.GetAll
{
    public class GetAllTeamsQuery : IQuery<List<TeamResult>>
    {
        public GetAllTeamsQuery() { }
    }
}
