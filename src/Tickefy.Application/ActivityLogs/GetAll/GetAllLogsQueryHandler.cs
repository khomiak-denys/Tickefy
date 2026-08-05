using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.ActivityLogs.GetAll
{
    public class GetAllLogsQueryHandler : IQueryHandler<GetAllLogsQuery, PaginationResult<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;

        public GetAllLogsQueryHandler(
            IActivityLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<PaginationResult<LogResult>> Handle(GetAllLogsQuery query, CancellationToken cancellationToken)
        {
            var pagedData = await _logRepository.GetAllAsync(query.PageNumber, query.PageSize, cancellationToken);
            
            var pagedLogs = pagedData.Items
                .Select(LogResult.FromEntity)
                .ToList();

            return PaginationResult<LogResult>.Create(pagedLogs, query.PageNumber, query.PageSize, pagedData.TotalCount);
        }
    }
}
