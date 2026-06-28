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
        /// Creates a JWT access token for the specified user identity.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="login">The user login.</param>
        /// <param name="role">The user role.</param>
        /// <returns>A signed JWT access token string.</returns>
        public Task<string> GetToken(Guid id, string login, UserRoles role);

        /// <summary>
        /// Generates a cryptographically random refresh token.
        /// </summary>
        /// <returns>A Base64-encoded random refresh token string.</returns>
        public string GenerateRefreshToken();
    }
}
