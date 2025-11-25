using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLog.Common;

namespace Tickefy.Application.ActivityLog.GetAll
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
