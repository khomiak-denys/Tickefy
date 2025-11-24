namespace Tickefy.Application.Ticket.Common
{
    public record AttachmentResult
    (
        string FilePath,
        string FileName,
        string ContentType, 
        long SizeBytes 
    );
}
