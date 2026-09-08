using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GetAllTicketsQueryHandler> _logger;

        public GetAllTicketsQueryHandler(
            ITicketRepository ticketRepository,
            ILogger<GetAllTicketsQueryHandler> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }
        public async Task<Result<PaginationResult<TicketResult>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var pagedData = await _ticketRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

            var pagedTickets = pagedData.Items
                .Select(TicketResult.FromEntity)
                .ToList();

            var result = PaginationResult<TicketResult>.Create(pagedTickets, request.Page, request.PageSize, pagedData.TotalCount);

            _logger.LogInformation("Retrieved {Count} tickets (Total: {TotalCount}, Page: {Page}, PageSize: {PageSize})", pagedTickets.Count, pagedData.TotalCount, request.Page, request.PageSize);

            return Result<PaginationResult<TicketResult>>.Success(result);
        }
    }
}
