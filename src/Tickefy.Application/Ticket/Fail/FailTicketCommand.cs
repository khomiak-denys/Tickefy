using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.Fail;

public class FailTicketCommand : ICommand<Unit>
{
    public UserId UserId { get; init; }
    public IEnumerable<string> Roles { get; init; }
    public TicketId TicketId { get; init; }
    public string Reason { get; init; }
}