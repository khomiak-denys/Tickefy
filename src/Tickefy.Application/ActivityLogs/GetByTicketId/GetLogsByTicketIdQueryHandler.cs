using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.ActivityLogs.GetByTicketId
{
    public class GetLogsByTicketIdQueryHandler : IQueryHandler<GetLogsByTicketIdQuery, PaginationResult<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;
        private readonly ILogger<GetLogsByTicketIdQueryHandler> _logger;

        public GetLogsByTicketIdQueryHandler(
            IActivityLogRepository logRepository,
            ILogger<GetLogsByTicketIdQueryHandler> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<PaginationResult<LogResult>> Handle(GetLogsByTicketIdQuery query, CancellationToken cancellationToken)
        {
            var pagedData = await _logRepository.GetByTicketIdAsync(query.TicketId, query.Page, query.PageSize, cancellationToken);

            var pagedLogs = pagedData.Items
                .Select(LogResult.FromEntity)
                .ToList();

            _logger.LogInformation("Retrieved {Count} activity logs for ticket {TicketId} (Total: {TotalCount}, Page: {Page}, PageSize: {PageSize})", pagedLogs.Count, query.TicketId.Value, pagedData.TotalCount, query.Page, query.PageSize);

            return PaginationResult<LogResult>.Create(pagedLogs, query.Page, query.PageSize, pagedData.TotalCount);
        }
    }
}
