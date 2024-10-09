using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Context
{
    public partial class HFDBNotificationServiceContext : DbContext
    {
        public HFDBNotificationServiceContext() { }

        public HFDBNotificationServiceContext(DbContextOptions<HFDBNotificationServiceContext> options)
            : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<UserNotificationPreference> UserNotificationPreferences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB_NotificationService; Username=postgres; Password=Abcd1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .Property(u => u.FullName)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role configuration
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleName)
                .IsRequired();

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
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .HasOne(unp => unp.User)
                .WithMany(u => u.NotificationPreferences)
                .HasForeignKey(unp => unp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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
