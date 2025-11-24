namespace Tickefy.API.Ticket.Responses
{
    public class AttachmentResponse(
        string FilePath,
        string FileName,
        string ContentType,
        long SizeBytes
        );
}
