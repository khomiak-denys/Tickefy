using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.AttachmentStatus;
using Tickefy.Application.Abstractions.Services;

namespace Tickefy.Application.Attachments.FinishUpload
{
    public class FinishFileUploadCommandHandler : ICommandHandler<FinishFileUploadCommand, Result>
    {
        private readonly ILogger<FinishFileUploadCommandHandler> _logger;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObjectStorageService _objectStorageService;

        public FinishFileUploadCommandHandler(
            ILogger<FinishFileUploadCommandHandler> logger,
            IAttachmentRepository attachmentRepository,
            IUnitOfWork unitOfWork,
            IObjectStorageService objectStorageService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _objectStorageService = objectStorageService ?? throw new ArgumentNullException(nameof(objectStorageService));
        }
        public async Task<Result> Handle(FinishFileUploadCommand command, CancellationToken cancellationToken)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(command.AttachmentId);

            if (attachment is null)
            {
                _logger.LogWarning("Attachment not found");
                return Result.Failure(new NotFoundError(nameof(attachment)));
            }

            if (attachment.Status == AttachmentStatus.Completed || attachment.Status == AttachmentStatus.Failed)
            {
                _logger.LogWarning("Unable to change attachment status");
                return Result.Failure(new ForbiddenError("Unable to change attachment status"));
            }

            var bytesFileSize = await _objectStorageService.GetContentLengthAsync(attachment.FilePath, cancellationToken);

            if (bytesFileSize is null)
            {
                _logger.LogWarning("Unable to finish upload for file {FilePath}. File doesn't exist in directory", attachment.FilePath);
                return Result.Failure(new NotFoundError(nameof(attachment)));
            }

            if (bytesFileSize.Value != command.SizeBytes)
            {
                _logger.LogWarning("Invalid size of file {FileSize}", command.SizeBytes);
                return Result.Failure(new InvalidArgumentError("Invalid size of file"));
            }

            attachment.FinishUpload();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
