using FluentValidation;

namespace Tickefy.Application.User.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator() 
        {
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.")
            .Matches("^[a-zA-Zа-яА-Я]+$").WithMessage("First name can contain only letters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters.")
                .Matches("^[a-zA-Zа-яА-Я]+$").WithMessage("Last name can contain only letters.");
        }
    }
}
