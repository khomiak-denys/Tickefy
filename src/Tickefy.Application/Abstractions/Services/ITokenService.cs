using Tickefy.Domain.Common.UserRole;

namespace Tickefy.Application.Abstractions.Services
{
    /// <summary>
    /// Creates authentication tokens for users.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Creates a token for the specified user identity.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="login">The user login.</param>
        /// <param name="role">The user role.</param>
        public Task<string> GetToken(Guid id, string login, UserRoles role);
    }
}
