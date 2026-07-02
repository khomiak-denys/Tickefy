using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.Fail;

public class FailTicketCommand : ICommand<Result>
{
    public UserId UserId { get; init; }
    public IEnumerable<string> Roles { get; init; }
    public TicketId TicketId { get; init; }
    public string Reason { get; init; }
}
