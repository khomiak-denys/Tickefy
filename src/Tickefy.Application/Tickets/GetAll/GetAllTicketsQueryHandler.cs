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
            var pagedData = await _ticketRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

            var pagedTickets = pagedData.Items
                .Select(TicketResult.FromEntity)
                .ToList();

            var result = PaginationResult<TicketResult>.Create(pagedTickets, request.PageNumber, request.PageSize, pagedData.TotalCount);

            return Result<PaginationResult<TicketResult>>.Success(result);
        }
    }
}
