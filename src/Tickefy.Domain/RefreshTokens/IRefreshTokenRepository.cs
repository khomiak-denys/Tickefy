using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.RefreshTokens;

/// <summary>
/// Defines contract requirements for persistence operations and lifecycle management of cryptographic refresh token entities.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Asynchronously locates and retrieves a stored refresh token entity by its raw cryptographic token string value.
    /// </summary>
    /// <param name="token">
    /// The exact string representation of the issued refresh token. Must be exact and case-sensitive; passing a malformed, null, or whitespace string will result in a null return value.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// A task representing the asynchronous read operation. The task result contains the matching <see cref="RefreshToken"/> instance if found; otherwise, <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// Callers should verify the expiration timestamp and revocation status of the returned entity before issuing new access tokens.
    /// </remarks>
    public Task<RefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages a newly generated refresh token entity for persistence in the underlying database upon transaction commit.
    /// </summary>
    /// <param name="refreshToken">
    /// The instantiated refresh token entity to store. Must be linked to a valid user ID and possess a future expiration date; passing an already tracked token will result in tracking conflicts.
    /// </param>
    /// <remarks>
    /// This is an in-memory staging operation; the token is not permanently written to storage until the associated unit of work commits the transaction.
    /// </remarks>
    public void Add(RefreshToken refreshToken);

    /// <summary>
    /// Asynchronously deletes any stored refresh token record matching the specified string value directly from the persistence store.
    /// </summary>
    /// <param name="token">
    /// The exact string value of the token to delete. If no matching token exists in storage, the operation completes silently without throwing an exception.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// A task representing the asynchronous deletion operation.
    /// </returns>
    /// <remarks>
    /// Unlike in-memory deletion staging, this method may execute an immediate database command or batch removal depending on repository implementation.
    /// </remarks>
    public Task DeleteByTokenAsync(string token, CancellationToken cancellationToken = default);


    /// <summary>
    /// Asynchronously removes a specific tracked refresh token entity instance from the underlying persistence context.
    /// </summary>
    /// <param name="refreshToken">
    /// The specific refresh token entity instance to remove. The entity should be currently tracked by the persistence context; attempting to delete an untracked instance may require prior attachment or result in no-op behavior.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// A task representing the asynchronous removal operation.
    /// </returns>
    public Task DeleteAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}
