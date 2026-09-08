using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Auth.SetPassword
{
    public class SetPasswordCommandHandler : ICommandHandler<SetPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<SetPasswordCommandHandler> _logger;

        public SetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork uow,
            ILogger<SetPasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Password change failed: user {UserId} not found", command.UserId.Value);
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId));
            }

            if (!_passwordHasher.VerifyPassword(command.OldPassword, user.PasswordHash))
            {
                _logger.LogWarning("Password change failed: invalid old password for user {UserId}", command.UserId.Value);
                return Result.Failure(new InvalidArgumentError("Invalid credentials"));
            }
            var passwordHash = _passwordHasher.HashPassword(command.NewPassword);
            user.UpdatePassword(passwordHash);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Password updated successfully for user {UserId}", user.Id.Value);

            return Result.Success();
        }
    }
}
