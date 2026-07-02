using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Auth.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Auth.Register
{
    /// <summary>
    /// Command to register a new user and return authentication tokens.
    /// </summary>
    public class RegisterUserCommand : ICommand<Result<LoginResult>>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }
    }
}
