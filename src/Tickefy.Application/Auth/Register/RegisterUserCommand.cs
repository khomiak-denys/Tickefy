using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Auth.Register
{
    public class RegisterUserCommand : ICommand<Result<Guid>>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }
    }
}
