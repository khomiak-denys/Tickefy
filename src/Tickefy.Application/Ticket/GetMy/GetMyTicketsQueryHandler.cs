using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.GetMy
{
    public class GetMyTicketsQueryHandler : IQueryHandler<GetMyTicketsQuery, Result<List<TicketResult>>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public GetMyTicketsQueryHandler(
            ITicketRepository ticketRepository,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<TicketResult>>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetByUserId(request.UserId);
            var result = _mapper.Map<List<TicketResult>>(tickets);

            return Result<List<TicketResult>>.Success(result);
        }
    }
}
