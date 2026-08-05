using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.ActivityLogs.GetByTicketId
{
    public class GetLogsByTicketIdQueryHandler : IQueryHandler<GetLogsByTicketIdQuery, PaginationResult<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;

        public GetLogsByTicketIdQueryHandler(
            IActivityLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<PaginationResult<LogResult>> Handle(GetLogsByTicketIdQuery query, CancellationToken cancellationToken)
        {
            var pagedData = await _logRepository.GetByTicketIdAsync(query.TicketId, query.PageNumber, query.PageSize, cancellationToken);
            
            var pagedLogs = pagedData.Items
                .Select(LogResult.FromEntity)
                .ToList();

            return PaginationResult<LogResult>.Create(pagedLogs, query.PageNumber, query.PageSize, pagedData.TotalCount);
        }
    }
}
