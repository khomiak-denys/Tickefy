using FluentValidation;

namespace Tickefy.Application.User.GetByLogin
{

    public class GetUserByLoginQueryValidator : AbstractValidator<GetUserByLoginQuery>
    {
        public GetUserByLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Login is required.")
                .Length(3, 50).WithMessage("Login must be between 3 and 50 characters.")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Login can contain only letters and digits.");
        }
    }
}