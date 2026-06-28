using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.RefreshToken;

/// <summary>
/// Provides persistence operations for refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Gets a token by its value.
    /// </summary>
    /// <param name="token">The value of the token.</param>
    /// <returns>The matching <see cref="RefreshToken"/>, or <see langword="null"/> if not found.</returns>
    public Task<RefreshToken?> GetToken(string token);

    /// <summary>
    /// Adds a token to the repository.
    /// </summary>
    /// <param name="refreshToken">The token to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Add(RefreshToken refreshToken);

    /// <summary>
    /// Deletes a token from the repository by its string value.
    /// </summary>
    /// <param name="token">The token string to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task DeleteByToken(string token);


    /// <summary>
    /// Deletes a refresh token entity from the repository.
    /// </summary>
    /// <param name="refreshToken">The token entity to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Delete(RefreshToken refreshToken);
}
