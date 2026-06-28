using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.RefreshToken;

public interface IRefreshTokenRepository
{
    /// <summary>
    /// Gets a token by its value.
    /// </summary>
    /// <param name="token">The value of the token.</param>
    public Task<RefreshToken?> GetToken(string token);

    /// <summary>
    /// Adds a token to the repository.
    /// </summary>
    /// <param name="refreshToken">The token to add.</param>
    public Task Add(RefreshToken refreshToken);

    /// <summary>
    /// Deletes token from repository.
    /// </summary>
    /// <param name="token">The token to delete.</param>
    public Task DeleteByToken(string token);


    /// <summary>
    /// Deletes token from repository.
    /// </summary>
    /// <param name="refreshToken">Token entity.</param>
    public Task Delete(RefreshToken refreshToken);
}
