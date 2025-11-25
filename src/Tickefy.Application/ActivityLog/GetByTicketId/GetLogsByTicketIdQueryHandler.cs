using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLog.Common;
using Tickefy.Domain.ActivityLog;

namespace Tickefy.Application.ActivityLog.GetByTicketId
{
    public class GetLogsByTicketIdQueryHandler : IQueryHandler<GetLogsByTicketIdQuery, List<LogResult>>
    {
        private readonly IActivityLogRepository _logRepository;
        private readonly IMapper _mapper;

        public GetLogsByTicketIdQueryHandler(
            IActivityLogRepository logRepository,
            IMapper mapper)
        {
            _logRepository = logRepository;
            _mapper = mapper;
        }
        public async Task<List<LogResult>> Handle(GetLogsByTicketIdQuery query, CancellationToken cancellationToken)
        {
            var logs = await _logRepository.GetByTicketIdAsync(query.TicketId);

            var result = _mapper.Map<List<LogResult>>(logs);

            return result;
        }
    }
}
