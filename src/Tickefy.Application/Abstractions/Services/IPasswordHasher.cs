namespace Tickefy.Application.Abstractions.Services
{
    /// <summary>
    /// Provides password hashing and verification operations.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a plain-text password.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        public string HashPassword(string password);

        /// <summary>
        /// Verifies a plain-text password against a password hash.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="hash">The password hash to compare against.</param>
        public bool VerifyPassword(string password, string hash);
    }
}
