using Tickefy.Domain.ActivityLog;
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
    }
}
