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

        public async Task<(int TotalCount, List<ActivityLog> Items)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ActivityLogs.AsNoTracking();
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(l => l.User)
                .OrderByDescending(l => l.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<ActivityLog> Items)> GetByTicketIdAsync(TicketId ticketId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ActivityLogs.Where(l => l.TicketId == ticketId).AsNoTracking();
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(l => l.User)
                .OrderByDescending(l => l.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (totalCount, items);
        }
    }
}
