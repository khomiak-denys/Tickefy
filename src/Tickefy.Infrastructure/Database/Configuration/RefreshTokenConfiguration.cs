using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickefy.Domain.RefreshTokens;
using Tickefy.Infrastructure.Extensions;

namespace Tickefy.Infrastructure.Database.Configuration
{
    internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasStronglyTypedIdConversion(a => a.Id);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .IsRequired();

            builder.HasIndex(t => t.Token).IsUnique();
        }
    }
}
