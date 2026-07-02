using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Teams.Delete
{
    public class DeleteTeamCommand : ICommand<Result>
    {
        public TeamId TeamId { get; init; }
        public UserId ManagerId { get; init; }
        public DeleteTeamCommand(TeamId teamId, UserId managerId)
        {
            TeamId = teamId;
            ManagerId = managerId;
        }
    }
}
