using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tickefy.Application.Auth.SetPassword
{
    public class SetPasswordCommandHandler : ICommandHandler<SetPasswordCommand>
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

        public async Task Handle(SetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(command.UserId);
            if (user == null) throw new NotFoundException(nameof(user), command.UserId);

            if (!_passwordHasher.VerifyPassword(command.OldPassword, user.PasswordHash))
            {
                throw new InvalidArgumentException("Invalid credentials");
            }
            var passwordHash = _passwordHasher.HashPassword(command.NewPassword);
            user.UpdatePassword(passwordHash);

            await _uow.SaveChangesAsync();
        }
    }
}
