using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Context
{
    public partial class HFDbContext : DbContext
    {
        public HFDbContext() { }

        public HFDbContext(DbContextOptions<HFDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Token> Tokens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB; Username=postgres; Password=Abcd1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r =>r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role configuration
            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Token configuration
            modelBuilder.Entity<Token>()
                .HasKey(t => t.TokenId);
            modelBuilder.Entity<Token>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Token>()
                .HasIndex(t => t.RefreshToken)
                .IsUnique();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
