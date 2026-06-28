using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.RefreshToken;

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

        /// <summary>
        /// Creates a refresh_token for the specified user identity.
        /// </summary>
        public string GenerateRefreshToken();
    }
}
