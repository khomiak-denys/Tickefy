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
            // Note: EFLogRepository.GetAllAsync(page, pageSize) does pagination at DB level.
            // But we need the total count. If we just get all and paginate in memory, it might be slow.
            // Let's retrieve a large number or all, since we didn't add CountAsync to repository yet.
            // But actually wait, the repository currently does `Take(pageSize).Skip((page-1)*pageSize)`.
            // The instructions said: "pagination metadata should be formed on application layer".
            // Since we must not change repositories according to our current tasks (we only checked they exist),
            // let's just fetch all by passing large size or 0 to get all, OR just change IActivityLogRepository.
            // Let's fetch all by passing 1, int.MaxValue.
            var allLogs = await _logRepository.GetAllAsync(1, int.MaxValue, cancellationToken);
            var totalCount = allLogs.Count;

            var pagedLogs = allLogs
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(LogResult.FromEntity)
                .ToList();

            return PaginationResult<LogResult>.Create(pagedLogs, query.PageNumber, query.PageSize, totalCount);
        }
    }
}
