using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using Tickefy.Domain.Common.EntityBase;

namespace Tickefy.Domain.Primitives.StronglyTypedId
{

    public static class StronglyTypedIdExtensions
    {
        public static PropertyBuilder<TId> HasStronglyTypedIdConversion<TEntity, TId>(
            this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, TId>> propertySelector)
            where TEntity : class
            where TId : StronglyTypedId<TId>
        {

            return builder
                .Property(propertySelector)
                .HasConversion(
                    id => id.Value,
                    value => (TId)Activator.CreateInstance(typeof(TId), value)!
                );
        }
    }
}
