using Tickefy.Application.Abstractions.Messaging;

namespace Tickefy.Application.Auth.Register
{
    public class RegisterUserCommand : ICommand<Guid>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }
    }
}
