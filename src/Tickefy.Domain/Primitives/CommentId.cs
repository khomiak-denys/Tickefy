using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Primitives
{
    public sealed record CommentId : StronglyTypedId<CommentId>
    {
        public CommentId(Guid value) : base(value) { }
        public CommentId() { }
    }
}