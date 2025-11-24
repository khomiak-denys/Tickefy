using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.GetAll
{
    public class GetAllTicketsQueryHandler : IQueryHandler<GetAllTicketsQuery, List<TicketResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public GetAllTicketsQueryHandler(
            ITicketRepository ticketRepository,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }
        public async Task<List<TicketResult>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetAll(); 
            var result = _mapper.Map<List<TicketResult>>(tickets);

            return result;
        }
    }
}
