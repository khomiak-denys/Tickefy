using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.AttachmentStatus;
using Tickefy.Application.Abstractions.Services;

namespace Tickefy.Application.Attachments.FailUpload
{
    public class FailFileUploadCommandHandler : ICommandHandler<FailFileUploadCommand, Result>
    {
        private readonly ILogger<FailFileUploadCommandHandler> _logger;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObjectStorageService _objectStorageService;

        public FailFileUploadCommandHandler(
            ILogger<FailFileUploadCommandHandler> logger,
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
        public async Task<Result> Handle(FailFileUploadCommand command, CancellationToken cancellationToken)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(command.AttachmentId);

            if (attachment is null)
            {
                _logger.LogWarning("Attachment not found");
                return Result.Failure(new NotFoundError(nameof(attachment)));
            }

            if (attachment.Status == AttachmentStatus.Failed || attachment.Status == AttachmentStatus.Completed)
            {
                _logger.LogWarning("Unable to change attachment status");
                return Result.Failure(new ForbiddenError("Unable to change attachment status"));
            }

            await _objectStorageService.DeleteFileAsync(attachment.FilePath, cancellationToken);

            attachment.FailUpload();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

