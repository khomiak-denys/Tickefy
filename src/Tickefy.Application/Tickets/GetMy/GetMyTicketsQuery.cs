using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.GetMy
{
    public class GetMyTicketsQuery : IQuery<Result<List<TicketResult>>>
    {
        public UserId UserId { get; init; }

        public GetMyTicketsQuery(UserId id)
        {
            UserId = id;
        }
    }
}
