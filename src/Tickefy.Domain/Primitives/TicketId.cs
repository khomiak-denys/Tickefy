using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Primitives
{
    public sealed record TicketId : StronglyTypedId<TicketId>
    {
        public TicketId(Guid value) : base(value) { }
        public TicketId() { }
    }
}