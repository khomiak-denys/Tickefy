using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.RemoveMember
{
    public class RemoveMemberCommandHandler : ICommandHandler<RemoveMemberCommand, Result>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        public RemoveMemberCommandHandler(
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            IUnitOfWork uow)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _uow = uow;
        }
        public async Task<Result> Handle(RemoveMemberCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.MemberId);
            if (user == null) return Result.Failure(new NotFoundError(nameof(user) + " " + command.MemberId.Value));
            if (user.Role == UserRoles.Admin || user.Role == UserRoles.Manager) return Result.Failure(new ForbiddenError("Cant remove manager or admin to team"));

            var team = await _teamRepository.GetByIdAsync(command.TeamId);

            if (team == null) return Result.Failure(new NotFoundError(nameof(team) + " " + command.ManagerId.Value));

            var manager = await _userRepository.GetByIdAsync(command.ManagerId);
            if (manager == null) return Result.Failure(new NotFoundError(nameof(manager) + " " + command.ManagerId.Value));
            if (team.ManagerId != command.ManagerId && manager.Role != UserRoles.Admin) return Result.Failure(new ForbiddenError("Become a manager to remove users"));

            var removeResult = team.RemoveMember(user);
            if (removeResult.IsFailure) return removeResult;

            user.SetRole(UserRoles.Requester);

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
