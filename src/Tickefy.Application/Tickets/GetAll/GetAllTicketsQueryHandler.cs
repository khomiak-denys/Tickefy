using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Tickets.GetAll
{
    public class GetAllTicketsQueryHandler : IQueryHandler<GetAllTicketsQuery, Result<PaginationResult<TicketResult>>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetAllTicketsQueryHandler(
            ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        public async Task<Result<PaginationResult<TicketResult>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetAllAsync(cancellationToken);
            var totalCount = tickets.Count();

            var pagedTickets = tickets
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(TicketResult.FromEntity)
                .ToList();

            var result = PaginationResult<TicketResult>.Create(pagedTickets, request.PageNumber, request.PageSize, totalCount);

            return Result<PaginationResult<TicketResult>>.Success(result);
        }
    }
}
