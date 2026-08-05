using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Teams;
using Tickefy.Infrastructure.Database;

namespace Tickefy.Infrastructure.Repositories
{
    public class EFTeamRepository : ITeamRepository
    {
        private readonly AppDbContext _dbContext;

        public EFTeamRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(Team team)
        {
            _dbContext.Add(team);
        }

        public void Delete(Team team)
        {
            _dbContext.Teams.Remove(team);
        }

        public async Task<(int TotalCount, List<Team> Items)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Teams;
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(t => t.Manager)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return (totalCount, items);
        }

        public async Task<Team?> GetByIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Teams.Where(t => t.Id == teamId)
                .Include(t => t.Manager)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(int TotalCount, List<Team> Items)> GetByMemberIdAsync(UserId memberId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Teams.Where(t => t.ManagerId == memberId || t.Members.Any(m => m.Id == memberId));

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(t => t.Manager)
                .Include(t => t.Members)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return (totalCount, items);
        }

        public async Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Teams.Where(t => t.Name == name)
                .Include(t => t.Manager)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
