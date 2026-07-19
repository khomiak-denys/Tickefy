using Tickefy.Application.Tickets.Common;
using Tickefy.Domain.Common.Content;

namespace Tickefy.API.Ticket.Responses
{
    public record AttachmentResponse(
        string Url,
        string FileName,
        string ContentType,
        long SizeBytes
        )
    {
        public static AttachmentResponse FromResult(AttachmentResult result) => new(
            result.Url,
            result.FileName,
            result.ContentType,
            result.SizeBytes
        );
    }
}
