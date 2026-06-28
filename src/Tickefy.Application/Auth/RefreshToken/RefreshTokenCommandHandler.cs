using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.RefreshToken;

namespace Tickefy.Application.Auth.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<LoginResult>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResult>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetToken(command.RefreshToken);

        if (token is null)
        {
            return Result<LoginResult>.Failure(new NotFoundError("Refresh token not found"));
        }

        if (token.Expires < DateTime.UtcNow)
        {
            await _refreshTokenRepository.Delete(token);
            return Result<LoginResult>.Failure(new ForbiddenError("Refresh token is expired"));
        }

        await _refreshTokenRepository.Delete(token);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newRefreshTokenEntity = Domain.RefreshToken.RefreshToken.Create(token.UserId, DateTime.UtcNow.AddDays(7), newRefreshToken);
        var accessToken = await _tokenService.GetToken(token.User.Id.Value, token.User.Login, token.User.Role);

        await _refreshTokenRepository.Add(newRefreshTokenEntity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResult>.Success(new LoginResult
        (
            token.User.Id.Value,
            token.User.FirstName,
            token.User.LastName,
            token.User.Login,
            accessToken,
            newRefreshTokenEntity.Token
        ));
    }
}
