using Tickefy.Domain.Common.AttachmentStatus;
using Tickefy.Domain.Common.Content;
using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;

namespace Tickefy.Domain.Attachments
{
    public class Attachment : EntityBase<AttachmentId>
    {
        public string FileName { get; private set; } = null!;
        public ContentType ContentType { get; private set; }
        public long SizeBytes { get; private set; }
        public TicketId TicketId { get; private set; } = null!;
        public Ticket Ticket { get; private set; } = null!;
        public AttachmentStatus Status { get; private set; } = AttachmentStatus.Pending;

        public string FilePath => $"tickets/{TicketId.Value}/attachments/{Id.Value}";

        private Attachment() { }

        public static Attachment Create(string fileName, ContentType contentType, long sizeBytes, TicketId ticketId)
        {
            var attachment = new Attachment(fileName, contentType, sizeBytes, ticketId);
            attachment.OnCreate();
            return attachment;
        }

        private Attachment(string fileName, ContentType contentType, long sizeBytes, TicketId ticketId)
        {
            FileName = fileName;
            ContentType = contentType;
            SizeBytes = sizeBytes;
            TicketId = ticketId;
        }

        public void FinishUpload()
        {
            Status = AttachmentStatus.Completed;
        }

        public void FailUpload()
        {
            Status = AttachmentStatus.Failed;
        }
    }
}
