using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;

namespace Tickefy.Application.ActivityLogs.GetAll
{
    public class GetAllLogsQueryHandler : IQueryHandler<GetAllLogsQuery, List<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;

        public GetAllLogsQueryHandler(
            IActivityLogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public async Task<List<LogResult>> Handle(GetAllLogsQuery query, CancellationToken cancellationToken)
        {
            var logs = await _logRepository.GetAllAsync(query.Page, query.PageSize, cancellationToken);

            return logs.Select(LogResult.FromEntity).ToList();
        }
    }
}
