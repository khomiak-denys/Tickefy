using Tickefy.Application.Ticket.Publish;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Ticket.Requests;

public class PublishTicketRequest
{
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime Deadline { get; init; }
    public PublishTicketCommand ToCommand(UserId userId, IEnumerable<string> roles, TicketId ticketId)
    {
        return new PublishTicketCommand
        {
            Title = Title,
            Description = Description,
            Deadline = Deadline,
            TicketId = ticketId,
            UserId = userId,
            Roles = roles
        };
    }
}