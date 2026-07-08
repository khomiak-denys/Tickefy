using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.GetAll
{
    public class GetAllTicketsQueryHandler : IQueryHandler<GetAllTicketsQuery, Result<List<TicketResult>>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetAllTicketsQueryHandler(
            ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        public async Task<Result<List<TicketResult>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetAllAsync(cancellationToken);
            var result = tickets.Select(TicketResult.FromEntity).ToList();

            return Result<List<TicketResult>>.Success(result);
        }
    }
}
