using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.Delete
{
    public class DeleteTeamCommandHandler : ICommandHandler<DeleteTeamCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        public DeleteTeamCommandHandler(
            ITeamRepository teamRepository, 
            IUserRepository userRepository,
            IUnitOfWork uow)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _uow = uow;
        }

        public async Task<Unit> Handle(DeleteTeamCommand command, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(command.TeamId);
            if (team == null) throw new NotFoundException(nameof(team), command.TeamId);

            if (team.ManagerId != command.ManagerId) throw new ForbiddenException("Not a manager role to delete team");

            var user = await _userRepository.GetByIdAsync(command.ManagerId);
            if (user == null) throw new NotFoundException(nameof(user), command.ManagerId);

            foreach (var usr in team.Members)
            {
                if (usr.Role != Domain.Common.UserRole.UserRoles.Admin)
                {
                    usr.SetRole(Domain.Common.UserRole.UserRoles.Requester);
                }
            }

            _teamRepository.Delete(team);

            await _uow.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
