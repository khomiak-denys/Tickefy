using FluentValidation;

namespace Tickefy.Application.ActivityLog.GetAll
{
    public class GetAllLogsQueryValidator : AbstractValidator<GetAllLogsQuery>
    {
        public GetAllLogsQueryValidator()
        {
            RuleFor(q => q.Page)
                .NotEmpty()
                    .WithMessage("Page is required.")
                .InclusiveBetween(1, 10000)
                    .WithMessage("Page must be between 1 and 10000.");

            RuleFor(q => q.PageSize)
                .NotEmpty()
                    .WithMessage("PageSize is required.")
                .InclusiveBetween(1, 1000)
                    .WithMessage("PageSize must be between 1 and 1000.");
        }
    }
}
