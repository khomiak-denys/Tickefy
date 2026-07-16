using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Primitives
{
    public sealed record TokenId : StronglyTypedId<TokenId>
    {
        public TokenId(Guid value) : base(value) { }
        public TokenId() { }
    }
}
