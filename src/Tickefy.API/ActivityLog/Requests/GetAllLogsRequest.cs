using Tickefy.Application.ActivityLogs.GetAll;

namespace Tickefy.API.ActivityLog.Requests
{
    public class GetAllLogsRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public GetAllLogsQuery ToQuery()
        {
            return new GetAllLogsQuery { Page = Page, PageSize = PageSize };
        }
    }
}
