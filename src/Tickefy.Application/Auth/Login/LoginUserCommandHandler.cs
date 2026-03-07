using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Сommon;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.User;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, Result<LoginResult>>
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

        public async Task<Result<LoginResult>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByLoginAsync(command.Login);
            if (existingUser == null)
            {
                return Result<LoginResult>.Failure(new NotFoundError("User not found"));
            }

            if (!_passwordHasher.VerifyPassword(command.Password, existingUser.PasswordHash))
            {
                return Result<LoginResult>.Failure(new InvalidArgumentError("Invalid credentials"));
            }

            var token = await _tokenService.GetToken(existingUser.Id.Value, existingUser.Login, existingUser.Role);

            return Result<LoginResult>.Success(new LoginResult
            (
                existingUser.Id.Value,
                existingUser.FirstName,
                existingUser.LastName,
                existingUser.Login,
                token
            ));
        }
    }
}
