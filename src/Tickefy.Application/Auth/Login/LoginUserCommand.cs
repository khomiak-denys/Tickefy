using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Auth.Сommon;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommand : ICommand<Result<LoginResult>>
    {
        public string Login { get; init; }
        public string Password { get; init; }
    }
}
