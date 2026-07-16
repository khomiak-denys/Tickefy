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

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetTokenAsync(command.RefreshToken, cancellationToken);

        if (existingToken is null)
        {
            return Result.Failure(new NotFoundError("Refresh token not found"));
        }

        await _refreshTokenRepository.DeleteAsync(existingToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
