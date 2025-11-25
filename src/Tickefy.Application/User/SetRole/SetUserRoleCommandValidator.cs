using FluentValidation;
using Tickefy.Domain.Common.UserRole;

namespace Tickefy.Application.User.SetRole
{
    public class SetUserRoleCommandValidator : AbstractValidator<SetUserRoleCommand>
    {
        public SetUserRoleCommandValidator()
        {
            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(BeValidRole)
                .WithMessage("Invalid role value");
        }

        private static bool BeValidRole(string role)
        {
            return Enum.TryParse<UserRoles>(role, ignoreCase: true, out _);
        }
    }
}
