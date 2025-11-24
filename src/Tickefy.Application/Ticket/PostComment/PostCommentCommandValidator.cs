using FluentValidation;

namespace Tickefy.Application.Ticket.PostComment
{
    public class PostCommentCommandValidator : AbstractValidator<PostCommentCommand>
    {
        public PostCommentCommandValidator() 
        {
            RuleFor(x => x.TicketId)
                .NotNull()
                .WithMessage("Ticket ID is required.");

            RuleFor(x => x.TicketId.Value.ToString())
                .Cascade(CascadeMode.Stop)
                .Must(id => id != string.Empty)
                .WithMessage("Ticket ID cannot be empty.")
                .Must(id => id.ToString().Length == 36)
                .WithMessage("Ticket ID must be 36 characters long.")
                .Matches("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$")
                .WithMessage("Ticket ID must match GUID format.")
                .When(x => x.TicketId != null);


            RuleFor(x => x.UserId)
                .NotNull()
                .WithMessage("Ticket ID is required.");
            
            RuleFor(x => x.UserId.Value.ToString())
                .Cascade(CascadeMode.Stop)
                .Must(id => id != string.Empty)
                .WithMessage("Ticket ID cannot be empty.")
                .Must(id => id.ToString().Length == 36)
                .WithMessage("Ticket ID must be 36 characters long.")
                .Matches("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$")
                .WithMessage("Ticket ID must match GUID format.")
                .When(x => x.UserId != null);

            RuleFor(x => x.Content)
                .MaximumLength(2000);
        }
    }
}
