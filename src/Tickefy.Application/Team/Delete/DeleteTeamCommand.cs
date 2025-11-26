using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Team.Delete
{
    public class DeleteTeamCommand : ICommand
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
