using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Tickets.GetAll
{
    public class GetAllTicketsQuery : IQuery<Result<List<TicketResult>>>
    {
        public GetAllTicketsQuery() { }
    }
}
