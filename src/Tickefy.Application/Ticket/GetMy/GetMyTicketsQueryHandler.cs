using AutoMapper;
using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.GetMy
{
    public class GetMyTicketsQueryHandler : IQueryHandler<GetMyTicketsQuery, List<TicketResult>>
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

        public async Task<List<TicketResult>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetByUserId(request.UserId);
            var result = _mapper.Map<List<TicketResult>>(tickets);

            return result;
        }
    }
}
