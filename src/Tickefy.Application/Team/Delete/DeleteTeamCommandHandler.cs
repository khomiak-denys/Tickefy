using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Team.Delete
{
    public class DeleteTeamCommandHandler : ICommandHandler<DeleteTeamCommand, Result>
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

        public async Task<Result> Handle(DeleteTeamCommand command, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
            if (team == null) return Result.Failure(new NotFoundError(nameof(team) + " " + command.TeamId));

            if (team.ManagerId != command.ManagerId) return Result.Failure(new ForbiddenError("Not a manager role to delete team"));

            var user = await _userRepository.GetByIdAsync(command.ManagerId, cancellationToken);
            if (user == null) return Result.Failure(new NotFoundError(nameof(user) + " " + command.ManagerId));

            foreach (var usr in team.Members)
            {
                if (usr.Role != Domain.Common.UserRole.UserRoles.Admin)
                {
                    usr.SetRole(Domain.Common.UserRole.UserRoles.Requester);
                }
            }

            _teamRepository.Delete(team);

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
