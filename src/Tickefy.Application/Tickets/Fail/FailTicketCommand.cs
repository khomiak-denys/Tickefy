using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.Fail;

public class FailTicketCommand : ICommand<Result>
{
    public required UserId UserId { get; init; }
    public required IEnumerable<string> Roles { get; init; }
    public required TicketId TicketId { get; init; }
    public required string Reason { get; init; }
}
