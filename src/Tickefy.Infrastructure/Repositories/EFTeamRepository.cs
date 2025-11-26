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

        public async Task<Team?> GetByIdAsync(TeamId teamId)
        {
            return await _dbContext.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
        }

        public async Task<Team?> GetByManagerIdAsync(UserId managerId)
        {
            return await _dbContext.Teams.Where(t => t.ManagerId == managerId).FirstOrDefaultAsync();
        }
    }
}
