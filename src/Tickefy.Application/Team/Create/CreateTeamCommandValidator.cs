using FluentValidation;

namespace Tickefy.Application.Team.Create
{
    public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        public CreateTeamCommandValidator() 
        {
            RuleFor(x => x.UserId)
            .NotNull()
            .WithMessage("Team leader ID cannot be null.");

            RuleFor(x => x.UserId.Value)
                .NotEmpty()
                .WithMessage("Team leader ID value cannot be empty.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Team name is required.")
                .MinimumLength(3)
                .WithMessage("Team name must be at least 3 characters long.")
                .MaximumLength(100)
                .WithMessage("Team name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Team description is required.")
                .MaximumLength(500)
                .WithMessage("Team description cannot exceed 500 characters.");

            RuleFor(x => x.Category)
            .NotNull()
            .WithMessage("Team category is required.")
            .IsInEnum()
            .WithMessage("Invalid team category specified.");
        }
    }
}
