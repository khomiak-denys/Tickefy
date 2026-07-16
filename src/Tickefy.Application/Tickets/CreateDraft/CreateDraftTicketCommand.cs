using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.CreateDraft;

public class CreateDraftTicketCommand : ICommand<Result>
{
    public required UserId UserId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTime Deadline { get; init; } = DateTime.Today;
}
