namespace Tickefy.Application.Tickets.Common
{
    public record AttachmentResult
    (
        string FilePath,
        string FileName,
        string ContentType,
        long SizeBytes
    );
}
