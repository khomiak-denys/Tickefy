using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickefy.Domain.Teams;
using Tickefy.Infrastructure.Extensions;

namespace Tickefy.Infrastructure.Database.Configuration
{
    internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasStronglyTypedIdConversion(a => a.Id);

            builder.HasIndex(t => t.Name).IsUnique();

            builder.HasOne(t => t.Manager)
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
