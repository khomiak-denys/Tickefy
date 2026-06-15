using FluentValidation;

namespace Tickefy.Application.Ticket.CreateDraft;

public class CreateDraftTicketCommandValidator : AbstractValidator<CreateDraftTicketCommand>
{
    public CreateDraftTicketCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Deadline)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
    }
}
