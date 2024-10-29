using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Context
{
    public partial class HFDBGroupServiceContext : DbContext
    {
        public HFDBGroupServiceContext() { }

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
