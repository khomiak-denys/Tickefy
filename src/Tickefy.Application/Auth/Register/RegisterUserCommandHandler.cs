using MediatR;
using Tickefy.Application.Abstractions.Messaging;

namespace Tickefy.Application.Auth.Register
{
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
    {
        public Task<Guid> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
