using System.Data;
using FluentValidation;

namespace Tickefy.Application.Attachments.FinishUpload
{
    public class FinishFileUploadCommandValidator : AbstractValidator<FinishFileUploadCommand>
    {
        private const long MaxFileSize = 10 * 1024 * 1024;
        public FinishFileUploadCommandValidator()
        {
            RuleFor(x => x.SizeBytes)
                .GreaterThan(0)
                .WithMessage("Size bytes must be greater than zero")
                .LessThan(MaxFileSize)
                .WithMessage("Size bytes must be less than or equal to 10MB");
        }
    }
}
