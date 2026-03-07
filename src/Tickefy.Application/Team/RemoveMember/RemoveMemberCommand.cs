using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Team.RemoveMember
{
    public class RemoveMemberCommand : ICommand<Result>
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
