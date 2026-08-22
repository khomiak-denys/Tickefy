using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickefy.Domain.Tickets;
using Tickefy.Infrastructure.Extensions;

namespace Tickefy.Infrastructure.Database.Configuration
{
    internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasStronglyTypedIdConversion(a => a.Id);

            builder.HasOne(t => t.Requester)
                .WithMany()
                .HasForeignKey(t => t.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AssignedTeam)
                .WithMany()
                .HasForeignKey(t => t.AssignedTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AssignedAgent)
                .WithMany()
                .HasForeignKey(t => t.AssignedAgentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
