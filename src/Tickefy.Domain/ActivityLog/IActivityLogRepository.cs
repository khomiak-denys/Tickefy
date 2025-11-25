using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.ActivityLog
{
    public interface IActivityLogRepository
    {
        void Add(ActivityLog log);
        Task<List<ActivityLog>> GetAllAsync(int page, int pageSize);
        Task<List<ActivityLog>> GetByTicketIdAsync(TicketId ticketId);
    }
}
