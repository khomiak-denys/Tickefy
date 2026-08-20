using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickefy.Domain.Attachments;
using Tickefy.Infrastructure.Extensions;

namespace Tickefy.Infrastructure.Database.Configuration
{
    internal sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasStronglyTypedIdConversion(a => a.Id);

            builder.HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(a => a.FilePath)
                .HasMaxLength(2048)
                .IsRequired();

            builder.Property(a => a.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(a => a.SizeBytes).IsRequired();
            builder.Property(a => a.ContentType).HasConversion<string>();
        }
    }
}
