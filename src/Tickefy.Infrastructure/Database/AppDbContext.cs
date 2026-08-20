using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Comments;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Primitives.StronglyTypedId;
using Tickefy.Infrastructure.Extensions;
using Tickefy.Domain.RefreshTokens;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Tickets;
using Tickefy.Domain.Users;

namespace Tickefy.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IgnoreAllTypedIds(modelBuilder);
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        private static void IgnoreAllTypedIds(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore(typeof(ActivityLogId));
            modelBuilder.Ignore(typeof(AttachmentId));
            modelBuilder.Ignore(typeof(CommentId));
            modelBuilder.Ignore(typeof(TeamId));
            modelBuilder.Ignore(typeof(TicketId));
            modelBuilder.Ignore(typeof(UserId));
            modelBuilder.Ignore(typeof(TokenId));
        }
    }
}
