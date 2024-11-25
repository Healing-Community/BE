using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Context
{
    public partial class HFDBGroupServiceContext : DbContext
    {
        public HFDBGroupServiceContext() { }

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<ApprovalQueue> ApprovalQueues { get; set; }
        public HFDBGroupServiceContext(DbContextOptions<HFDBGroupServiceContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.xbclwyxnkwbpcnumpwzc; Password=ProjectHealing@1234");

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB_GroupService; Username=postgres; Password=Abcd1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.GroupId);
                entity.Property(e => e.GroupName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedByUserId).IsRequired();
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.UserId });
                entity.HasOne<Group>()
                      .WithMany()
                      .HasForeignKey(e => e.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.UserId).IsRequired();
            });

            // ApprovalQueue entity configuration
            modelBuilder.Entity<ApprovalQueue>(entity =>
            {
                entity.HasKey(e => e.QueueId); // Primary key
                entity.Property(e => e.GroupId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.RequestedAt).IsRequired();
                entity.Property(e => e.IsApproved)
                      .IsRequired()
                      .HasDefaultValue(false); // Default: Not approved
                entity.HasOne<Group>()
                      .WithMany()
                      .HasForeignKey(e => e.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
