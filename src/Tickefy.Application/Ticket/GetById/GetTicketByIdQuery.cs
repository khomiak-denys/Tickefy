using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.GetById
{
    public class GetTicketByIdQuery : IQuery<TicketDetailsResult>
    {
        public UserId UserId { get; init; }
        public List<string> Roles { get; init; }
        public TicketId TicketId { get; init; }
        

        public GetTicketByIdQuery(UserId userId, List<string> roles, TicketId ticketId)
        {
            UserId = userId;
            Roles = roles;
            TicketId = ticketId;
        }
    }
}
