using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Primitives
{
    public sealed record TeamId : StronglyTypedId<TeamId>
    {
        public TeamId(Guid value) : base(value) { }
        public TeamId() { }
    }
}