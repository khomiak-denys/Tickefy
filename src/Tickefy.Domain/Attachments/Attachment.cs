using Tickefy.Domain.Common.Content;
using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;

namespace Tickefy.Domain.Attachments
{
    public class Attachment : EntityBase<AttachmentId>
    {
        public string FilePath { get; private set; } = null!;
        public string FileName { get; private set; } = null!;
        public ContentType ContentType { get; private set; }
        public long SizeBytes { get; private set; }
        public TicketId TicketId { get; private set; } = null!;
        public Ticket Ticket { get; private set; } = null!;

        private Attachment() { }

        public static Attachment Create(string filePath, string fileName, ContentType contentType, long sizeBytes, TicketId ticketId)
        {
            var attachment = new Attachment(filePath, fileName, contentType, sizeBytes, ticketId);
            attachment.OnCreate();
            return attachment;
        }

        private Attachment(string filePath, string fileName, ContentType contentType, long sizeBytes, TicketId ticketId)
        {
            FilePath = filePath;
            FileName = fileName;
            ContentType = contentType;
            SizeBytes = sizeBytes;
            TicketId = ticketId;
        }
    }
}
