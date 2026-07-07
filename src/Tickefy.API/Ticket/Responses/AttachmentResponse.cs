using Tickefy.Application.Tickets.Common;

namespace Tickefy.API.Ticket.Responses
{
    public record AttachmentResponse(
        string FilePath,
        string FileName,
        string ContentType,
        long SizeBytes
        )
    {
        public static AttachmentResponse FromResult(AttachmentResult result) => new(
            result.FilePath,
            result.FileName,
            result.ContentType,
            result.SizeBytes
        );
    }
}
