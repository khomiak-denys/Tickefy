using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.CreateDraft;

public class CreateDraftTicketCommand : ICommand<Result>
{
    public UserId UserId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime Deadline { get; init; } = DateTime.Today;
}
