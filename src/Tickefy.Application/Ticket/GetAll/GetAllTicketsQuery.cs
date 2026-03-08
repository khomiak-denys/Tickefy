using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Ticket.GetAll
{
    public class GetAllTicketsQuery : IQuery<Result<List<TicketResult>>>
    {
        public GetAllTicketsQuery() { }
    }
}
