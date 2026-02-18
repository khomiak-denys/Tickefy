using Tickefy.Domain.Primitives;

namespace Tickefy.API.Ticket.Requests;

public class CreateDraftTicketRequest
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? Deadline { get; init; }

    public CreateDraftTicketCommand ToCommand(UserId userId)
    {
        return new CreateDraftTicketCommand
        {
            UserId = userId,
            Title = Title,
            Description = Description,
            Deadline = Deadline
        };
    }
}