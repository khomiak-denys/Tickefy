using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Primitives
{
    public sealed record AttachmentId : StronglyTypedId<AttachmentId>
    {
        public AttachmentId(Guid value) : base(value) { }
        public AttachmentId() { }
    }
}