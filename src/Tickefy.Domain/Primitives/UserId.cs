using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Primitives
{
    public sealed record UserId : StronglyTypedId<UserId>
    {
        public UserId(Guid value) : base(value) { }
        public UserId() { }
    }
}