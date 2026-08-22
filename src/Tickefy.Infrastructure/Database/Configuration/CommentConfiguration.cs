using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickefy.Domain.Comments;
using Tickefy.Infrastructure.Extensions;

namespace Tickefy.Infrastructure.Database.Configuration
{
    internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasStronglyTypedIdConversion(a => a.Id);

            builder.HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.Content).HasMaxLength(4000);
        }
    }
}
