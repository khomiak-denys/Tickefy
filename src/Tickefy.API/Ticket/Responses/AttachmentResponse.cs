namespace Tickefy.API.Ticket.Responses
{
    public record AttachmentResponse(
        string FilePath,
        string FileName,
        string ContentType,
        long SizeBytes
        );
}
