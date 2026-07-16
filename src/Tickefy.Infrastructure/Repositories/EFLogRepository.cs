using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.ActivityLogs;
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

        public async Task<List<ActivityLog>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ActivityLogs
                .Include(l => l.User)
                .AsNoTracking()
                .OrderByDescending(l => l.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ActivityLog>> GetByTicketIdAsync(TicketId ticketId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ActivityLogs.Where(l => l.TicketId == ticketId).ToListAsync(cancellationToken);
        }
    }
}
