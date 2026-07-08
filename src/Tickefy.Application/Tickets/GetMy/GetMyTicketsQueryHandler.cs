using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.GetMy
{
    public class GetMyTicketsQueryHandler : IQueryHandler<GetMyTicketsQuery, Result<List<TicketResult>>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetMyTicketsQueryHandler(
            ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<Result<List<TicketResult>>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var result = tickets.Select(TicketResult.FromEntity).ToList();

            return Result<List<TicketResult>>.Success(result);
        }
    }
}
