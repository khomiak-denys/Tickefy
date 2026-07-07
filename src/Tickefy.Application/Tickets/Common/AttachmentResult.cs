using Tickefy.Domain.Attachments;

namespace Tickefy.Application.Tickets.Common
{
    public record AttachmentResult
    (
        string FilePath,
        string FileName,
        string ContentType,
        long SizeBytes
    )
    {
        public static AttachmentResult FromEntity(Attachment attachment) => new(
            attachment.FilePath,
            attachment.FileName,
            attachment.ContentType.ToString(),
            attachment.SizeBytes
        );
    }
}
