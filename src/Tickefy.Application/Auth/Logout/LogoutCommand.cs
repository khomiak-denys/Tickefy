using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Auth.Logout;

public class LogoutCommand : ICommand<Result>
{
    public string RefreshToken { get; init; }
    public LogoutCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
