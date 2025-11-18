using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Primitives
{
    public sealed record ActivityLogId : StronglyTypedId<ActivityLogId>
    {
        public ActivityLogId(Guid value) : base(value) { }
        public ActivityLogId() { }
    }
}