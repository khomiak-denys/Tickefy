using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.AddMember
{
    public class AddMemberCommandHandler(
        IUserRepository userRepository, 
        ITeamRepository teamRepository, 
        IUnitOfWork uow) : ICommandHandler<AddMemberCommand, Result>
    {
        public async Task<Result> Handle(AddMemberCommand command, CancellationToken cancellationToken)
        {
            var user = await  userRepository.GetByLoginAsync(command.MemberLogin);
            if (user == null) return Result.Failure(new NotFoundError(nameof(user) + " " + command.MemberLogin));
            if (user.Role == UserRoles.Admin || user.Role == UserRoles.Manager) return Result.Failure(new ForbiddenError("Cant add manager or admin to team"));

            var team = await teamRepository.GetByIdAsync(command.TeamId);

            if (team == null) return Result.Failure(new NotFoundError(nameof(team) + " " + command.ManagerId.Value));

            var manager = await userRepository.GetByIdAsync(command.ManagerId);
            if (manager == null) return Result.Failure(new NotFoundError(nameof(manager) + " " + command.ManagerId.Value));
            if (team.ManagerId != command.ManagerId && manager.Role != UserRoles.Admin) return Result.Failure(new ForbiddenError("Become a manager to add users"));

            team.AddMember(user);
            user.SetRole(UserRoles.Agent);

            await uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
