using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Attachment;
using Tickefy.Domain.Comment;
using Tickefy.Domain.Primitives;
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
            modelBuilder.Entity<ActivityLog>().HasKey(a => a.Id);
            modelBuilder.Entity<Attachment>().HasKey(a => a.Id);
            modelBuilder.Entity<Comment>().HasKey(a => a.Id);
            modelBuilder.Entity<Team>().HasKey(a => a.Id);
            modelBuilder.Entity<Ticket>().HasKey(a => a.Id);
            modelBuilder.Entity<User>().HasKey(a => a.Id);

            modelBuilder.Entity<ActivityLog>().HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<Attachment>().HasStronglyTypedIdConversion(a => a.Id);
            modelBuilder.Entity<Comment>().HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<Team>().HasStronglyTypedIdConversion(a => a.Id);
            modelBuilder.Entity<Ticket>().HasStronglyTypedIdConversion(a => a.Id);

            modelBuilder.Entity<User>().HasStronglyTypedIdConversion(a => a.Id);

            //ACTIVITY LOG
            modelBuilder.Entity<ActivityLog>()
                    .HasOne<Ticket>()
                    .WithMany()
                    .HasForeignKey(a => a.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivityLog>()
                   .HasOne<User>()
                   .WithMany()
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ActivityLog>().HasIndex(a => a.TicketId);

            //ATTACHMENT
            modelBuilder.Entity<Attachment>()
                   .HasOne(a => a.Ticket)
                   .WithMany(t => t.Attachments) 
                   .HasForeignKey(a => a.TicketId)
                   .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Attachment>()
                   .Property(a => a.FilePath)
                   .HasMaxLength(2048) 
                   .IsRequired();

            modelBuilder.Entity<Attachment>()
                   .Property(a => a.FileName)
                   .HasMaxLength(255)
                   .IsRequired();

            modelBuilder.Entity<Attachment>().Property(a => a.SizeBytes).IsRequired();
            modelBuilder.Entity<Attachment>().Property(a => a.ContentType).HasConversion<string>();

            //COMMENT
            modelBuilder.Entity<Comment>()
                   .HasOne(c => c.Ticket) 
                   .WithMany(t => t.Comments) 
                   .HasForeignKey(c => c.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                   .HasOne(c => c.User)
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>().Property(c => c.Content).HasMaxLength(4000);

            //TEAM
            modelBuilder.Entity<Team>().HasIndex(t => t.Name).IsUnique();

            //TICKET
            modelBuilder.Entity<Ticket>()
                   .HasOne(t => t.Requester)
                   .WithMany()
                   .HasForeignKey(t => t.RequesterId)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                   .HasOne(t => t.AssignedTeam)
                   .WithMany()
                   .HasForeignKey(t => t.AssignedTeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                   .HasOne(t => t.AssignedAgent)
                   .WithMany()
                   .HasForeignKey(t => t.AssignedAgentId)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                   .HasMany(t => t.Attachments)
                   .WithOne()
                   .HasForeignKey(a => a.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);

            //USER
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<User>()
                   .HasOne(u => u.Team)
                   .WithMany(t => t.Members)
                   .HasForeignKey(u => u.TeamId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}