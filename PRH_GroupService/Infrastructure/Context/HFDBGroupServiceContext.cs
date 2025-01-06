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
        public virtual DbSet<GroupCreationRequest> GroupCreationRequests { get; set; }
        public HFDBGroupServiceContext(DbContextOptions<HFDBGroupServiceContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.xbclwyxnkwbpcnumpwzc; Password=ProjectHealing@1234");

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB_GroupService; Username=postgres; Password=Abcd1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // **Groups Configuration**
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.GroupId);
                entity.Property(e => e.GroupName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.GroupName).IsUnique(); // Unique constraint on GroupName
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedByUserId).IsRequired();

                // Relationship: Groups -> UserGroups
                entity.HasMany<UserGroup>()
                      .WithOne()
                      .HasForeignKey(ug => ug.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship: Groups -> ApprovalQueues
                entity.HasMany<ApprovalQueue>()
                      .WithOne()
                      .HasForeignKey(aq => aq.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // **UserGroups Configuration**
            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.UserId }); // Composite key
                entity.Property(e => e.RoleInGroup).HasMaxLength(20).HasDefaultValue("User");
                entity.Property(e => e.JoinedAt).IsRequired();
            });

            // **ApprovalQueues Configuration**
            modelBuilder.Entity<ApprovalQueue>(entity =>
            {
                entity.HasKey(e => e.QueueId); // Primary Key
                entity.Property(e => e.GroupId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.RequestedAt).IsRequired();
                entity.Property(e => e.IsApproved).HasDefaultValue(false); // Default: Not approved
            });

            // **GroupCreationRequests Configuration**
            modelBuilder.Entity<GroupCreationRequest>(entity =>
            {
                entity.HasKey(gc => gc.GroupRequestId);
                entity.Property(gc => gc.RequestedAt).HasDefaultValueSql("NOW()");
                entity.Property(gc => gc.IsApproved).HasDefaultValue(null);
                entity.Property(gc => gc.ApprovedAt).IsRequired(false);
                entity.Property(gc => gc.ApprovedById).IsRequired(false);
                entity.Property(gc => gc.CoverImg).HasMaxLength(255).IsRequired(false);
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
