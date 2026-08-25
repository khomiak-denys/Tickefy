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

        private string Extension => Path.GetExtension(FileName);
        public string FilePath => $"tickets/{TicketId.Value}/attachments/{Id.Value}{Extension}";

        private Attachment() { }

        public static Attachment Create(string fileName, long sizeBytes, TicketId ticketId)
        {
            var attachment = new Attachment(fileName, sizeBytes, ticketId);
            attachment.OnCreate();
            return attachment;
        }

        private Attachment(string fileName, long sizeBytes, TicketId ticketId)
        {
            FileName = fileName;
            ContentType = GetFileContentType(Extension);
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

        public static bool IsExtensionAllowed(string fileExtension) => AllowedFileTypes.ContainsKey(fileExtension.ToLowerInvariant());

        private static readonly Dictionary<string, ContentType> AllowedFileTypes = new Dictionary<string, ContentType>
        {
            { ".txt", ContentType.Document },
            { ".pdf", ContentType.Document },
            { ".docx", ContentType.Document },
            { ".zip", ContentType.Archive },
            { ".rar", ContentType.Archive },
            { ".jpeg", ContentType.Photo },
            { ".jpg", ContentType.Photo },
            { ".png", ContentType.Photo },
            { ".mp4", ContentType.Video }
        };

        private static ContentType GetFileContentType(string fileExtension)
        {
            if (IsExtensionAllowed(fileExtension))
            {
                return AllowedFileTypes[fileExtension.ToLowerInvariant()];
            }

            throw new InvalidOperationException(nameof(GetFileContentType));
        }
    }

}
