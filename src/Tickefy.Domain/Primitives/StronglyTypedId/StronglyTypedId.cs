namespace Tickefy.Domain.Primitives.StronglyTypedId
{
    public abstract partial record StronglyTypedId<T> where T : StronglyTypedId<T>
    {
        protected StronglyTypedId(Guid id)
        {
            Value = id;
        }
        protected StronglyTypedId()
        {
            Value = Guid.NewGuid();
        }
        public Guid Value { get; }
    }
}
