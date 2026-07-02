using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.ActivityLogs.Common;
using Tickefy.Domain.ActivityLogs;

namespace Tickefy.Application.ActivityLogs.GetByTicketId
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
            var logs = await _logRepository.GetByTicketIdAsync(query.TicketId, cancellationToken);

            var result = _mapper.Map<List<LogResult>>(logs);

            return result;
        }
    }
}
