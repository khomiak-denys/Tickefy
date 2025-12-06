using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.AddMember
{
    public class AddMemberCommandHandler : ICommandHandler<AddMemberCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _uow;

        public AddMemberCommandHandler(
            IUserRepository userRepository, 
            ITeamRepository teamRepository, 
            IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _uow = uow;
        }

        public async Task Handle(AddMemberCommand command, CancellationToken cancellationToken)
        {
            var user = await  _userRepository.GetByIdAsync(command.UserId);
            if (user == null) throw new NotFoundException(nameof(user), command.UserId.Value);
            if (user.Role == UserRoles.Admin || user.Role == UserRoles.Manager) throw new ForbiddenException("Cant add manager or admin to team");

            var team = await _teamRepository.GetByIdAsync(command.TeamId);

            if (team == null) throw new NotFoundException(nameof(team), command.ManagerId.Value);

            var manager = await _userRepository.GetByIdAsync(command.ManagerId);
            if (manager == null) throw new NotFoundException(nameof(manager), command.ManagerId.Value);
            if (team.ManagerId != command.ManagerId && manager.Role != UserRoles.Admin) throw new ForbiddenException("Become a manager to add users");

            team.AddMember(user);
            user.SetRole(UserRoles.Agent);

            await _uow.SaveChangesAsync();
        }
    }
}
