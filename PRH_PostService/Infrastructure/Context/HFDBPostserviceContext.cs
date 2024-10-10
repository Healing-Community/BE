using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public partial class HFDBPostserviceContext : DbContext
    {
        public HFDBPostserviceContext() { }

        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public HFDBPostserviceContext(DbContextOptions<HFDBPostserviceContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.kulssrgvnfgpytdjvmyy; Password=ProjectHealing@1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary key
            modelBuilder.Entity<Post>().HasKey(p => p.Id);
            modelBuilder.Entity<Post>().HasIndex(p => p.Title).IsUnique();
            modelBuilder.Entity<Category>().HasKey(p => p.Id);
            modelBuilder.Entity<Category>().HasIndex(c=>c.Name).IsUnique();
            // Relationship
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId);
               

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
