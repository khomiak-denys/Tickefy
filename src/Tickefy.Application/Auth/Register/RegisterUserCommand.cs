using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Auth.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Auth.Register
{
    /// <summary>
    /// Represents an immutable transactional command request to register a new user identity within the system and generate initial authentication credentials.
    /// </summary>
    public class RegisterUserCommand : ICommand<Result<LoginResult>>
    {
        /// <summary>
        /// Gets the given name of the registering user. Must satisfy non-empty length and character validation constraints.
        /// </summary>
        public string FirstName { get; init; }

        /// <summary>
        /// Gets the surname or family name of the registering user. Used for display and formal identification purposes.
        /// </summary>
        public string LastName { get; init; }

        /// <summary>
        /// Gets the unique account identifier or username requested by the registering user. Must be unique across all active accounts; case sensitivity is determined by database collation rules.
        /// </summary>
        public string Login { get; init; }

        /// <summary>
        /// Gets the unhashed plain-text secret key provided for account authentication. Must satisfy minimum complexity and length requirements before hashing.
        /// </summary>
        public string Password { get; init; }
    }
}
