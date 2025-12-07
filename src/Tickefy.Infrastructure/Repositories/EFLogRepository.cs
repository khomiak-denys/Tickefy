using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Primitives;
using Tickefy.Infrastructure.Database;

namespace Tickefy.Infrastructure.Repositories
{
    public class EFLogRepository : IActivityLogRepository
    {
        private readonly AppDbContext _dbContext;
        public EFLogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ActivityLog log)
        {
            _dbContext.ActivityLogs.Add(log);
        }

        public async Task<List<ActivityLog>> GetAllAsync(int page, int pageSize)
        {
            return await _dbContext.ActivityLogs
                .Include(l => l.User)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ActivityLog>> GetByTicketIdAsync(TicketId ticketId)
        {
            return await _dbContext.ActivityLogs.Where(l => l.TicketId == ticketId).ToListAsync();
        }
    }
}
