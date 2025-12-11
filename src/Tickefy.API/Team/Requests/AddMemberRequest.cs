using Tickefy.Application.Team.AddMember;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Team.Requests
{
    public class AddMemberRequest
    {
        public string Login { get; init; }
        
        public AddMemberCommand ToCommand(TeamId teamId, UserId userId)
        {
            return new AddMemberCommand(Login, userId, teamId);
        }
    }
}