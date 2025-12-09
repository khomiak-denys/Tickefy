using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.AddMember
{
    public class AddMemberCommandHandler(
        IUserRepository userRepository, 
        ITeamRepository teamRepository, 
        IUnitOfWork uow) : ICommandHandler<AddMemberCommand>
    {
        public async Task Handle(AddMemberCommand command, CancellationToken cancellationToken)
        {
            var user = await  userRepository.GetByLoginAsync(command.MemberLogin);
            if (user == null) throw new NotFoundException(nameof(user), command.MemberLogin);
            if (user.Role == UserRoles.Admin || user.Role == UserRoles.Manager) throw new ForbiddenException("Cant add manager or admin to team");

            var team = await teamRepository.GetByIdAsync(command.TeamId);

            if (team == null) throw new NotFoundException(nameof(team), command.ManagerId.Value);

            var manager = await userRepository.GetByIdAsync(command.ManagerId);
            if (manager == null) throw new NotFoundException(nameof(manager), command.ManagerId.Value);
            if (team.ManagerId != command.ManagerId && manager.Role != UserRoles.Admin) throw new ForbiddenException("Become a manager to add users");

            team.AddMember(user);
            user.SetRole(UserRoles.Agent);

            await uow.SaveChangesAsync();
        }
    }
}
