using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Auth.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Auth.RefreshToken;

public class RefreshTokenCommand : ICommand<Result<LoginResult>>
{
    public string RefreshToken { get; init; }

    public  RefreshTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
