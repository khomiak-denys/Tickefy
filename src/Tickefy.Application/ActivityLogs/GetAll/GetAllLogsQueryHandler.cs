using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.ActivityLogs.GetAll
{
    public class GetAllLogsQueryHandler : IQueryHandler<GetAllLogsQuery, PaginationResult<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;
        private readonly ILogger<GetAllLogsQueryHandler> _logger;

        public GetAllLogsQueryHandler(
            IActivityLogRepository logRepository,
            ILogger<GetAllLogsQueryHandler> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<PaginationResult<LogResult>> Handle(GetAllLogsQuery query, CancellationToken cancellationToken)
        {
            var pagedData = await _logRepository.GetAllAsync(query.Page, query.PageSize, cancellationToken);

            var pagedLogs = pagedData.Items
                .Select(LogResult.FromEntity)
                .ToList();

            _logger.LogInformation("Retrieved {Count} activity logs (Total: {TotalCount}, Page: {Page}, PageSize: {PageSize})", pagedLogs.Count, pagedData.TotalCount, query.Page, query.PageSize);

            return PaginationResult<LogResult>.Create(pagedLogs, query.Page, query.PageSize, pagedData.TotalCount);
        }
    }
}
