using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public partial class HFDBPostserviceContext : DbContext
    {
        public HFDBPostserviceContext() { }

        public virtual DbSet<Post> Costs { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public HFDBPostserviceContext(DbContextOptions<HFDBPostserviceContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB_PostService; Username=postgres; Password=Abcd1234");

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
                .WithMany(c => c.posts)
                .HasForeignKey(p => p.CategoryId);
               

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
