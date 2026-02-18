using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Ticket.CreateDraft;

public class CreateDraftTicketCommand : ICommand<Unit>
{
    public UserId UserId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime Deadline { get; init; } = DateTime.Today;
}