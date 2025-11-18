using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Attachment;
using Tickefy.Domain.Comment;
using Tickefy.Domain.Primitives.StronglyTypedId;
using Tickefy.Domain.Team;
using Tickefy.Domain.Ticket;
using Tickefy.Domain.User;

namespace Tickefy.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Comment> Comments  { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLog>()
               .HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<Attachment>()
                .HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<Comment>()
                .HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<Team>()
                .HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<Ticket>()
                .HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<User>()
                .HasStronglyTypedIdConversion(a => a.Id);
        }
    }
}