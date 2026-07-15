using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Common.EntityBase
{
    public class EntityBase<T> where T : StronglyTypedId<T>, new()
    {
        public T Id { get; private init; }
        public DateTime Created { get; private set; }
        public DateTime? Modified { get; private set; }

        protected EntityBase(T id)
        {
            if (id is null || id.Value == Guid.Empty)
            {
                throw new ArgumentException("Id must be valid.");
            }

            Id = id;
        }

        protected EntityBase()
        {
            Id = new T();
        }

        protected void OnCreate()
        {
            Created = DateTime.UtcNow;
        }
        protected void OnModify()
        {
            Modified = DateTime.UtcNow;
        }
    }
}
