using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Common.EntityBase
{
    public class EntityBase<T>  where T : StronglyTypedId<T>
    {
        public T Id { get; protected set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }


        protected EntityBase(T id)
        {
            if (id is null || id.Value == Guid.Empty)
                throw new ArgumentException("Id must be valid.");

            Id = id;
        }
        protected EntityBase() { }

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