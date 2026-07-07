using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;

namespace Tickefy.Application.ActivityLogs.GetByTicketId
{
    public class GetLogsByTicketIdQueryHandler : IQueryHandler<GetLogsByTicketIdQuery, List<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;

        public GetLogsByTicketIdQueryHandler(
            IActivityLogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public async Task<List<LogResult>> Handle(GetLogsByTicketIdQuery query, CancellationToken cancellationToken)
        {
            var logs = await _logRepository.GetByTicketIdAsync(query.TicketId, cancellationToken);

            return logs.Select(LogResult.FromEntity).ToList();
        }
    }
}
