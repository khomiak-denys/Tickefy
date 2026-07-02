using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.Publish;

public class PublishTicketCommand : ICommand<Result>
{
    public UserId UserId { get; init; }
    public IEnumerable<string> Roles { get; init; }
    public TicketId TicketId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime Deadline { get; init; }
}
