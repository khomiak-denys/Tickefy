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
        public SetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _uow = uow;
        }

        public async Task<Result> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null) return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId));

            if (!_passwordHasher.VerifyPassword(command.OldPassword, user.PasswordHash))
            {
                return Result.Failure(new InvalidArgumentError("Invalid credentials"));
            }
            var passwordHash = _passwordHasher.HashPassword(command.NewPassword);
            user.UpdatePassword(passwordHash);

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
