using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.RefreshTokens;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, Result<LoginResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResult>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByLoginAsync(command.Login, cancellationToken);
            if (existingUser == null)
            {
                return Result<LoginResult>.Failure(new NotFoundError("User not found"));
            }

            if (!_passwordHasher.VerifyPassword(command.Password, existingUser.PasswordHash))
            {
                return Result<LoginResult>.Failure(new InvalidArgumentError("Invalid credentials"));
            }

            var token = await _tokenService.GetTokenAsync(existingUser.Id.Value, existingUser.Login, existingUser.Role, cancellationToken);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = Domain.RefreshTokens.RefreshToken.Create(existingUser.Id, DateTime.UtcNow.AddDays(7), refreshToken);

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<LoginResult>.Success(new LoginResult
            (
                existingUser.Id.Value,
                existingUser.FirstName,
                existingUser.LastName,
                existingUser.Login,
                token,
                refreshToken
            ));
        }
    }
}
