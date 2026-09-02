using System.Data;
using FluentValidation;

namespace Tickefy.Application.Attachments.FailUpload
{
    public class FailFileUploadCommandValidator : AbstractValidator<FailFileUploadCommand>
    {
        private const long MaxFileSize = 10 * 1024 * 1024;
        public FailFileUploadCommandValidator()
        {
            RuleFor(x => x.SizeBytes)
                .GreaterThan(0)
                .WithMessage("Size bytes must be greater than zero")
                .LessThan(MaxFileSize)
                .WithMessage("Size bytes must be less than or equal to 10MB");
        }
    }
}

