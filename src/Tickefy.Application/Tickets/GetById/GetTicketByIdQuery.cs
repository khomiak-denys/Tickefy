using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.GetById
{
    public class GetTicketByIdQuery : IQuery<Result<TicketDetailsResult>>
    {
        public UserId UserId { get; init; }
        public List<string> Roles { get; init; }
        public TicketId TicketId { get; init; }

        public GetTicketByIdQuery(
            UserId userId,
            List<string> roles,
            TicketId ticketId)
        {
            UserId = userId;
            Roles = roles;
            TicketId = ticketId;
        }
    }
}
