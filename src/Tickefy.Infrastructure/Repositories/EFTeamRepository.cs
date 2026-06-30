using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Team;
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

        public async Task<List<Team>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Teams
                .Include(t => t.Manager)
                .ToListAsync(cancellationToken);
        }

        public async Task<Team?> GetByIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Teams.Where(t => t.Id == teamId)
                .Include(t => t.Manager)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Team>> GetByMemberIdAsync(UserId userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Teams.Where(t => t.ManagerId == userId || t.Members.Any(m => m.Id == userId))
                .Include(t => t.Manager)
                .Include(t => t.Members)
                .ToListAsync(cancellationToken);
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
