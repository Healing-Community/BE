using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Context
{
    public partial class HFDBNotificationServiceContext : DbContext
    {
        public HFDBNotificationServiceContext() { }

        public HFDBNotificationServiceContext(DbContextOptions<HFDBNotificationServiceContext> options)
            : base(options) { }

        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<UserNotificationPreference> UserNotificationPreferences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com;Database=postgres;Username=postgres.sozqnfhxkfabtlhdkvas;Password=ProjectHealing@1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {          
            // Notification configuration
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(n => n.Message)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(n => n.UpdatedAt)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.NotificationType)
                .WithMany(nt => nt.Notifications)
                .HasForeignKey(n => n.NotificationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // NotificationType configuration
            modelBuilder.Entity<NotificationType>()
                .HasKey(nt => nt.NotificationTypeId);

            modelBuilder.Entity<NotificationType>()
                .HasIndex(nt => nt.Name)
                .IsUnique();

            modelBuilder.Entity<NotificationType>()
                .Property(nt => nt.Name)
                .IsRequired();

            modelBuilder.Entity<NotificationType>()
                .Property(nt => nt.Description)
                .IsRequired(false);

            // UserNotificationPreference configuration
            modelBuilder.Entity<UserNotificationPreference>()
                .HasKey(unp => new { unp.UserId, unp.NotificationTypeId });

            modelBuilder.Entity<UserNotificationPreference>()
                .HasOne(unp => unp.NotificationType)
                .WithMany(nt => nt.UserPreferences)
                .HasForeignKey(unp => unp.NotificationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserNotificationPreference>()
                .Property(unp => unp.IsSubscribed)
                .IsRequired();          

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
