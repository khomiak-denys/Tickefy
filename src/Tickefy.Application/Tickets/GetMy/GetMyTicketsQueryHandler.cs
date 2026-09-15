using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Tickets.GetMy
{
    public class GetMyTicketsQueryHandler : IQueryHandler<GetMyTicketsQuery, Result<PaginationResult<TicketResult>>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<GetMyTicketsQueryHandler> _logger;

        public GetMyTicketsQueryHandler(
            ITicketRepository ticketRepository,
            ILogger<GetMyTicketsQueryHandler> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<Result<PaginationResult<TicketResult>>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            var pagedData = await _ticketRepository.GetByUserIdAsync(request.UserId, request.Page, request.PageSize, cancellationToken);

            var pagedTickets = pagedData.Items
                .Select(TicketResult.FromEntity)
                .ToList();

            var result = PaginationResult<TicketResult>.Create(pagedTickets, request.Page, request.PageSize, pagedData.TotalCount);

            return Result<PaginationResult<TicketResult>>.Success(result);
        }
    }
}
