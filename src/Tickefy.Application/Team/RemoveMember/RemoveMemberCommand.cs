using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Team;

namespace Tickefy.Application.Team.RemoveMember
{
    public class RemoveMemberCommand : ICommand
    {
        public UserId MemberId { get; init; }
        public UserId ManagerId { get; init; }
        public TeamId TeamId { get; init; }

        public RemoveMemberCommand(UserId memberId, UserId managerId, TeamId teamId)
        {
            MemberId = memberId;
            ManagerId = managerId;
            TeamId = teamId;
        }
    }
}
