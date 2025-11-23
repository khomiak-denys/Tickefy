using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Common;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.User;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByLoginAsync(command.Login, cancellationToken);
            if (existingUser == null)
            {
                throw new NotFoundException("User not found");
            }

            if (!_passwordHasher.VerifyPassword(command.Password, existingUser.PasswordHash))
            {
                throw new InvalidArgumentException("Invalid credentials");
            }

            var token = await _tokenService.GetToken(existingUser.Id.Value, existingUser.Login, existingUser.Role);

            return new LoginResult
            (
                existingUser.Id.Value,
                existingUser.FirstName,
                existingUser.LastName,
                existingUser.Login,
                token
            );
            
        }
    }
}
