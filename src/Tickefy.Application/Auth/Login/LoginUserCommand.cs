using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Auth.Common;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommand : ICommand<LoginResult>
    {
        public string Login { get; init; }
        public string Password { get; init; }
    }
}
