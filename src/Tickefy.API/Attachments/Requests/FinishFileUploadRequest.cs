using Tickefy.Domain.Primitives;
using Tickefy.Application.Attachments.FinishUpload;

namespace Tickefy.API.Attachments.Requests
{
    public class FinishFileUploadRequest
    {
        public required long SizeBytes { get; init; }

        public FinishFileUploadCommand ToCommand(AttachmentId attachmentId)
        {
            return new FinishFileUploadCommand
            {
                AttachmentId = attachmentId,
                SizeBytes = SizeBytes
            };
        }
    }
}
