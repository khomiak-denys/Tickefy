using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Attachments
{
    /// <summary>
    /// Defines contract requirements for persistence operations and lifecycle management of file attachment domain entities.
    /// </summary>
    public interface IAttachmentRepository
    {
        Task AddAsync(Attachment attachment);
        Task<Attachment?> GetByIdAsync(AttachmentId id);
        Task DeleteAsync(AttachmentId id);
        void Update(Attachment attachment);
    }
}
