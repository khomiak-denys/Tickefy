using Tickefy.Domain.Primitives.StronglyTypedId;

namespace Tickefy.Domain.Common.EntityBase
{
    public class EntityBase<T>  where T : StronglyTypedId<T>
    {
        public T Id { get; private set; } = default!;
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

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