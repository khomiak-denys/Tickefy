using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;

namespace Tickefy.Application.ActivityLogs.GetAll
{
    public class GetAllLogsQuery : IQuery<List<LogResult>>
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public GetAllLogsQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
