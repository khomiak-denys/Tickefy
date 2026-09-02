using Tickefy.Application.Attachments.FailUpload;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Attachments.Requests
{
    public class FailFileUploadRequest
    {
        public long SizeBytes { get; init; }

        public FailFileUploadCommand ToCommand(AttachmentId attachmentId)
        {
            return new FailFileUploadCommand
            {
                AttachmentId = attachmentId,
                SizeBytes = SizeBytes
            };
        }
    }
}
