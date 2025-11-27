using Tickefy.Application.ActivityLog.GetAll;

namespace Tickefy.API.ActivityLog.Requests
{
    public class GetAllLogsRequest
    {
        public required int Page { get; init; }
        public required int PageSize { get; init; }

        public  GetAllLogsQuery ToQuery()
        {
            return new GetAllLogsQuery(Page, PageSize);
        }
    }
}
