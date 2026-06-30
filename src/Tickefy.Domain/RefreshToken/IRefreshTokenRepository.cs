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
    public Task<RefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a token to the repository.
    /// </summary>
    /// <param name="refreshToken">The token to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a token from the repository by its string value.
    /// </summary>
    /// <param name="token">The token string to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task DeleteByTokenAsync(string token, CancellationToken cancellationToken = default);


    /// <summary>
    /// Deletes a refresh token entity from the repository.
    /// </summary>
    /// <param name="refreshToken">The token entity to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task DeleteAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}
