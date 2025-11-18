using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Attachment
{
    public class Attachment : EntityBase<AttachmentId>
    {
        public string FilePath { get; set; } = "";       
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = ""; 
        public long SizeBytes { get; set; }
        public TicketId? RequestId { get; set; }
    }
}
