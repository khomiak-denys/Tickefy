using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickefy.Domain.Primitives.StronglyTypedId;
using System.Linq.Expressions;
using Tickefy.Domain.Common.EntityBase;

namespace Tickefy.Infrastructure.Extensions
{
    public static class StronglyTypedIdExtensions
    {
        public static PropertyBuilder<TId> HasStronglyTypedIdConversion<TEntity, TId>(
            this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, TId>> propertySelector)
            where TEntity : EntityBase<TId>
            where TId : StronglyTypedId<TId>, new()
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
