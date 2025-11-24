namespace Tickefy.Application.Ticket.Common
{
    public class AttachmentResult
    {
        string FilePath { get; init; }
        string FileName { get; init; }
        string ContentType { get; init; }
        long SizeBytes { get; init; }
    }
}
