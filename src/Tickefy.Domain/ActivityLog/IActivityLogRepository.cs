namespace Tickefy.Domain.ActivityLog
{
    public interface IActivityLogRepository
    {
        void Add(ActivityLog log);
    }
}
