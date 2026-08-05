using Tickefy.Application.ActivityLogs.GetAll;

namespace Tickefy.API.ActivityLog.Requests
{
    public class GetAllLogsRequest
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public GetAllLogsQuery ToQuery()
        {
            return new GetAllLogsQuery { PageNumber = PageNumber, PageSize = PageSize };
        }
    }
}
