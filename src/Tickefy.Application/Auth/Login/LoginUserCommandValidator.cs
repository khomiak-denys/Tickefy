using FluentValidation;

namespace Tickefy.Application.Auth.Login
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Login is required.")
                .Length(3, 50).WithMessage("Login must be between 3 and 50 characters.")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Login can contain only letters and digits.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")
                .WithMessage("Password must contain at least one letter and one number.");
        }
    }
}
