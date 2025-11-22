using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Auth.Common;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginResult>
    {
        public Task<LoginResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
