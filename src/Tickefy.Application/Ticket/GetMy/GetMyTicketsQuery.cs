using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.GetMy
{
    public class GetMyTicketsQuery : IQuery<List<TicketResult>>
    {
        public UserId UserId { get; init; }

        public GetMyTicketsQuery(UserId id) 
        {
            UserId = id;
        }
    }
}
