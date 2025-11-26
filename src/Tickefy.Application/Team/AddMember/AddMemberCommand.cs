using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Team.AddMember
{
    public class AddMemberCommand : ICommand
    {
        public UserId UserId { get; init; }
        public UserId ManagerId { get; init; }  
        public TeamId TeamId { get; init; }
        
        public AddMemberCommand(UserId userId, UserId managerId, TeamId teamId)
        {
            UserId = userId;
            ManagerId = managerId;
            TeamId = teamId;
        }
    }
}
