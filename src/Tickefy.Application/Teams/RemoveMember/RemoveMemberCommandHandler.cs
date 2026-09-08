using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Teams.RemoveMember
{
    public class RemoveMemberCommandHandler : ICommandHandler<RemoveMemberCommand, Result>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<RemoveMemberCommandHandler> _logger;

        public RemoveMemberCommandHandler(
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            IUnitOfWork uow,
            ILogger<RemoveMemberCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }
        public async Task<Result> Handle(RemoveMemberCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.MemberId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Remove member failed: user {MemberId} not found", command.MemberId.Value);
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.MemberId.Value));
            }

            if (user.Role == UserRoles.Admin || user.Role == UserRoles.Manager)
            {
                _logger.LogWarning("Remove member failed: user {MemberId} has role {Role} and cannot be removed", command.MemberId.Value, user.Role);
                return Result.Failure(new ForbiddenError("Cant remove manager or admin to team"));
            }

            var team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
            if (team == null)
            {
                _logger.LogWarning("Remove member failed: team {TeamId} not found", command.TeamId.Value);
                return Result.Failure(new NotFoundError(nameof(team) + " " + command.ManagerId.Value));
            }

            var manager = await _userRepository.GetByIdAsync(command.ManagerId, cancellationToken);
            if (manager == null)
            {
                _logger.LogWarning("Remove member failed: manager {ManagerId} not found", command.ManagerId.Value);
                return Result.Failure(new NotFoundError(nameof(manager) + " " + command.ManagerId.Value));
            }

            if (team.ManagerId != command.ManagerId && manager.Role != UserRoles.Admin)
            {
                _logger.LogWarning("Remove member failed: user {ManagerId} is not manager of team {TeamId} or admin", command.ManagerId.Value, command.TeamId.Value);
                return Result.Failure(new ForbiddenError("Become a manager to remove users"));
            }

            var removeResult = team.RemoveMember(user);
            if (removeResult.IsFailure)
            {
                _logger.LogWarning("Remove member failed from team {TeamId}: {ErrorMessage}", command.TeamId.Value, removeResult.Error.Message);
                return removeResult;
            }

            user.SetRole(UserRoles.Requester);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {MemberId} removed from team {TeamId} by manager {ManagerId}", command.MemberId.Value, command.TeamId.Value, command.ManagerId.Value);

            return Result.Success();
        }
    }
}
