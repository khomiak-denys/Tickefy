using Tickefy.Domain.User;
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

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.Users
                .Include(u => u.Team)
                .ThenInclude(t => t.Manager)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(UserId id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(t => t.Id == id); 
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}
