using Tickefy.Domain.Users;
using Tickefy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Primitives;

namespace Tickefy.Infrastructure.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public EFUserRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }
        public void Add(User user)
        {
            _dbContext.Users.Add(user);
        }

        public void Delete(User user)
        {
            _dbContext.Users.Remove(user);
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.Team!)
                .ThenInclude(t => t.Manager)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
        }
    }
}
