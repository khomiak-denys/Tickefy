using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.SetRole
{
    public class SetUserRoleCommandHandler : ICommandHandler<SetUserRoleCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<SetUserRoleCommandHandler> _logger;

        public SetUserRoleCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork uow,
            ILogger<SetUserRoleCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }
        public async Task<Result> Handle(SetUserRoleCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Set user role failed: user {UserId} not found", command.UserId.Value);
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId.ToString()));
            }

            UserRoles role;
            var result = Enum.TryParse<UserRoles>(command.Role, ignoreCase: true, out role);

            if (!result)
            {
                _logger.LogWarning("Invalid role {Role} provided for user {UserId}, defaulting to {DefaultRole}", command.Role, command.UserId.Value, UserRoles.Requester);
                role = UserRoles.Requester;
            }

            if (user.Role == UserRoles.Admin)
            {
                _logger.LogWarning("Set user role failed: cannot change role for Admin user {UserId}", command.UserId.Value);
                return Result.Failure(new ForbiddenError("Admin role cant be changed"));
            }

            user.SetRole(role);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role for user {UserId} updated to {Role}", command.UserId.Value, role);

            return Result.Success();
        }
    }
}
