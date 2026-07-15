using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Auth.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommand : ICommand<Result<LoginResult>>
    {
        public required string Login { get; init; }
        public required string Password { get; init; }
    }
}
