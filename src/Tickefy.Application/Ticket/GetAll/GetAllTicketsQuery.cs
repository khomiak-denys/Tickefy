using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;

namespace Tickefy.Application.Ticket.GetAll
{
    public class GetAllTicketsQuery : IQuery<List<TicketResult>>
    {
        public GetAllTicketsQuery() { }
    }
}
