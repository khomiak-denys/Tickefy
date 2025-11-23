using Tickefy.Domain.Common.ContentType;
using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Attachment
{
    public class Attachment : EntityBase<AttachmentId>
    {
        public string FilePath { get; private set; }      
        public string FileName { get; private set; } 
        public ContentType ContentType { get; private set; }
        public long SizeBytes { get; private set; }
        public TicketId TicketId { get; private set; }
        public Domain.Ticket.Ticket Ticket { get; private set; }

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
