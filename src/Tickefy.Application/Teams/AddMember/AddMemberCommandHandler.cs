using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Teams.AddMember
{
    public class AddMemberCommandHandler(
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IUnitOfWork uow,
        ILogger<AddMemberCommandHandler> logger) : ICommandHandler<AddMemberCommand, Result>
    {
        public async Task<Result> Handle(AddMemberCommand command, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByLoginAsync(command.MemberLogin, cancellationToken);
            if (user == null)
            {
                logger.LogWarning("Add member failed: user with login {MemberLogin} not found", command.MemberLogin);
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.MemberLogin));
            }

            if (user.Role == UserRoles.Admin || user.Role == UserRoles.Manager)
            {
                logger.LogWarning("Add member failed: user {MemberLogin} has role {Role} and cannot be added to team", command.MemberLogin, user.Role);
                return Result.Failure(new ForbiddenError("Cant add manager or admin to team"));
            }

            var team = await teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
            if (team == null)
            {
                logger.LogWarning("Add member failed: team {TeamId} not found", command.TeamId.Value);
                return Result.Failure(new NotFoundError(nameof(team) + " " + command.ManagerId.Value));
            }

            var manager = await userRepository.GetByIdAsync(command.ManagerId, cancellationToken);
            if (manager == null)
            {
                logger.LogWarning("Add member failed: manager {ManagerId} not found", command.ManagerId.Value);
                return Result.Failure(new NotFoundError(nameof(manager) + " " + command.ManagerId.Value));
            }

            if (team.ManagerId != command.ManagerId && manager.Role != UserRoles.Admin)
            {
                logger.LogWarning("Add member failed: user {ManagerId} is not manager of team {TeamId} or admin", command.ManagerId.Value, command.TeamId.Value);
                return Result.Failure(new ForbiddenError("Become a manager to add users"));
            }

            team.AddMember(user);
            user.SetRole(UserRoles.Agent);

            await uow.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User {MemberLogin} added to team {TeamId} by manager {ManagerId}", command.MemberLogin, command.TeamId.Value, command.ManagerId.Value);

            return Result.Success();
        }
    }
}
