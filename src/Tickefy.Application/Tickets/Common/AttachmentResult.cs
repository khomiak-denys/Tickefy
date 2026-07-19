using Tickefy.Domain.Attachments;
using Tickefy.Domain.Common.Content;

namespace Tickefy.Application.Tickets.Common
{
    public record AttachmentResult
    (
        string Url,
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
