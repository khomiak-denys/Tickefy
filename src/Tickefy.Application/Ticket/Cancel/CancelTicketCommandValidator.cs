using FluentValidation;

namespace Tickefy.Application.Ticket.Cancel
{
    public class CancelTicketCommandValidator : AbstractValidator<CancelTicketCommand>
    {
        public CancelTicketCommandValidator() 
        {
            RuleFor(x => x.TicketId)
                .NotNull()
                .WithMessage("Ticket ID is required.");

            RuleFor(x => x.TicketId.Value.ToString())
                .Cascade(CascadeMode.Stop)
                .Must(id => id != string.Empty)
                .WithMessage("Ticket ID cannot be empty.")
                .Must(id => id.Length == 36)
                .WithMessage("Ticket ID must be 36 characters long.")
                .Matches("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$")
                .WithMessage("Ticket ID must match GUID format.")
                .When(x => x.TicketId != null);
        }
    }
}
