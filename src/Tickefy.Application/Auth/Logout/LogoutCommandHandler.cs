using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.RefreshTokens;

namespace Tickefy.Application.Auth.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetTokenAsync(command.RefreshToken, cancellationToken);

        if (existingToken is null)
        {
            _logger.LogWarning("Logout attempted with nonexistent refresh token");
            return Result.Failure(new NotFoundError("Refresh token not found"));
        }

        await _refreshTokenRepository.DeleteAsync(existingToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged out successfully, refresh token revoked");

        return Result.Success();
    }
}
