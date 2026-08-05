using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.ActivityLogs.GetAll
{
    public class GetAllLogsQuery : IQuery<PaginationResult<LogResult>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public GetAllLogsQuery() { }
    }
}
