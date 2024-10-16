using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public partial class HFDBPostserviceContext : DbContext
    {
        public HFDBPostserviceContext() { }

        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Reaction> Reactions { get; set; }
        public virtual DbSet<ReactionType> ReactionTypes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public HFDBPostserviceContext(DbContextOptions<HFDBPostserviceContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.kulssrgvnfgpytdjvmyy; Password=ProjectHealing@1234");

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB_PostService; Username=postgres; Password=Abcd1234");


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

            // Reaction
            modelBuilder.Entity<Reaction>().HasKey(r => r.ReactionId);
            modelBuilder.Entity<Reaction>().HasOne(r => r.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(r => r.PostId);
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.ReactionType)
                .WithMany(rt => rt.Reactions)
                .HasForeignKey(r => r.ReactionTypeId);

            // ReactionType
            modelBuilder.Entity<ReactionType>()
                .HasKey(rt => rt.ReactionTypeId);

            // Comment
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.CommentId);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentId);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
