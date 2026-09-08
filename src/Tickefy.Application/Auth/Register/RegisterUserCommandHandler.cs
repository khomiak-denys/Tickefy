using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.RefreshTokens;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Auth.Register
{
    /// <summary>
    /// Coordinates the orchestration of user account registration, including uniqueness validation, password cryptographic hashing, domain entity persistence, and token issuance.
    /// </summary>
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<LoginResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork uow,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _uow = uow;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Result<LoginResult>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByLoginAsync(command.Login, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: user with login {Login} already exists", command.Login);
                return Result<LoginResult>.Failure(new AlreadyExistsError("User already exists"));
            }

            var passwordHash = _passwordHasher.HashPassword(command.Password);
            var user = Domain.Users.User.Create(command.FirstName, command.LastName, command.Login, passwordHash);

            _userRepository.Add(user);

            var token = _tokenService.GetToken(user.Id.Value, user.Login, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = Domain.RefreshTokens.RefreshToken.Create(user.Id, DateTime.UtcNow.AddDays(7), refreshToken);
            _refreshTokenRepository.Add(refreshTokenEntity);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Login} registered successfully with ID {UserId}", user.Login, user.Id.Value);

            return Result<LoginResult>.Success(new LoginResult(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Login,
                token,
                refreshToken
            ));
        }
    }
}
