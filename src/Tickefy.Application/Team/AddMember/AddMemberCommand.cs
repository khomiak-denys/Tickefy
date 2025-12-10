using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Team.AddMember
{
    public class AddMemberCommand : ICommand<Unit>
    {
        public string MemberLogin { get; init; }
        public UserId ManagerId { get; init; }  
        public TeamId TeamId { get; init; }
        
        public AddMemberCommand(string memberLogin, UserId managerId, TeamId teamId)
        {
            MemberLogin = memberLogin;
            ManagerId = managerId;
            TeamId = teamId;
        }
    }
}
