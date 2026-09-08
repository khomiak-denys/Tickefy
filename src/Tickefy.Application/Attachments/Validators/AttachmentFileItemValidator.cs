using FluentValidation;
using Tickefy.Application.Tickets.Create.Dto;
using Tickefy.Domain.Attachments;

namespace Tickefy.Application.Attachments.Validators
{
    public class AttachmentFileItemValidator : AbstractValidator<AttachmentFileItem>
    {
        private const long MaxFileSize = 10 * 1024 * 1024;

        public AttachmentFileItemValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("File name is required")
                .Must(x => Attachment.IsExtensionAllowed(Path.GetExtension(x)))
                .WithMessage("File type is not allowed");

            RuleFor(x => x.SizeBytes)
                .GreaterThan(0)
                .WithMessage("Size bytes must be greater than zero")
                .LessThan(MaxFileSize)
                .WithMessage("Size bytes must be less than or equal to 10MB");

            RuleFor(x => x.ClientFileId).NotEmpty();
        }
    }
}

