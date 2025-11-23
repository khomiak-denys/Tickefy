using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Domain.User;
using Tickefy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tickefy.Infrastructure.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public EFUserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void Add(User user)
        {
            _appDbContext.Users.Add(user);
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
        }
    }
}
