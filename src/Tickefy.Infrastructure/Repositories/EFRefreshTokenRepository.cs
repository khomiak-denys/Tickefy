using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.RefreshToken;
using Tickefy.Infrastructure.Database;

namespace Tickefy.Infrastructure.Repositories;

public class EFRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext;

    public EFRefreshTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetToken(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens.Where(r => r.Token == token)
            .Include(r => r.User)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task Add(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task DeleteByToken(string token, CancellationToken cancellationToken = default)
    {
        var existingToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
        if (existingToken is null)
        {
            return;
        }

        _dbContext.RefreshTokens.Remove(existingToken);
    }

    public async Task Delete(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Remove(refreshToken);
        await Task.CompletedTask;
    }
}
