using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Domain.User;
using Tickefy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
        }
    }
}
