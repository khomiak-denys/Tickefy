using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLog.Common;
using Tickefy.Domain.ActivityLog;

namespace Tickefy.Application.ActivityLog.GetAll
{
    public class GetAllLogsQueryHandler : IQueryHandler<GetAllLogsQuery, List<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;
        private readonly IMapper _mapper;
        
        public GetAllLogsQueryHandler(
            IActivityLogRepository logRepository,
            IMapper mapper)
        {
            _logRepository = logRepository;
            _mapper = mapper;
        }
        public async Task<List<LogResult>> Handle(GetAllLogsQuery query, CancellationToken cancellationToken)
        {
            var logs = await _logRepository.GetAllAsync(query.Page, query.PageSize);

            var result = _mapper.Map<List<LogResult>>(logs);

            return result;
        }
    }
}
