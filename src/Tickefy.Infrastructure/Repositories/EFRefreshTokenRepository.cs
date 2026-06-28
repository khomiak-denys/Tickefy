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

    public async Task<RefreshToken?> GetToken(string token)
    {
        return await _dbContext.RefreshTokens.Where(r => r.Token == token)
            .Include(r => r.User)
            .FirstOrDefaultAsync();
    }

    public async Task Add(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task DeleteByToken(string token)
    {
        var existingToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        if (existingToken is null)
        {
            return;
        }

        _dbContext.RefreshTokens.Remove(existingToken);
    }

    public async Task Delete(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Remove(refreshToken);
    }
}
