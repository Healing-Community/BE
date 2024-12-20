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
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<BookmarkPost> BookmarkPosts { get; set; }
        public DbSet<Share> Shares { get; set; }
        public HFDBPostserviceContext(DbContextOptions<HFDBPostserviceContext> options) : base(options) { }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.kulssrgvnfgpytdjvmyy; Password=ProjectHealing@1234");

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB_PostService; Username=postgres; Password=Abcd1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary key
            modelBuilder.Entity<Post>().HasKey(p => p.PostId);
            modelBuilder.Entity<Post>().HasIndex(p => p.Title).IsUnique();
            modelBuilder.Entity<Category>().HasKey(p => p.CategoryId);
            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Bookmark>().HasKey(b => b.BookmarkId);
            modelBuilder.Entity<BookmarkPost>().HasKey(bp => bp.BookmarkPostId);
            modelBuilder.Entity<Share>().HasKey(s => s.ShareId);
            //make sure that the combination of PostId and BookmarkId is unique
            modelBuilder.Entity<BookmarkPost>().HasIndex(bp => new { bp.PostId, bp.BookmarkId }).IsUnique();
            // Relationship
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId);

            // Reaction
            modelBuilder.Entity<Reaction>().HasKey(r => r.ReactionId);
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.ReactionType)
                .WithMany(rt => rt.Reactions)
                .HasForeignKey(r => r.ReactionTypeId);

            // ReactionType
            modelBuilder.Entity<ReactionType>().HasKey(rt => rt.ReactionTypeId);

            // Comment
            modelBuilder.Entity<Comment>().HasKey(c => c.CommentId);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Report 
            modelBuilder.Entity<Report>().HasKey(r => r.ReportId);
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Reports)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Report>()
                .HasOne(r => r.ReportType)
                .WithMany(rt => rt.Reports)
                .HasForeignKey(r => r.ReportTypeId);

            // ReportType 
            modelBuilder.Entity<ReportType>().HasKey(rt => rt.ReportTypeId);
            // user preference
            modelBuilder.Entity<UserPreference>().HasKey(up => up.Id);
            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.Category)
                .WithMany(c => c.UserPreferences)
                .HasForeignKey(up => up.CategoryId);
            OnModelCreatingPartial(modelBuilder);
            // Bookmark : One bookmark has many bookmark posts
            modelBuilder.Entity<Bookmark>()
                .HasMany(b => b.BookmarkPosts)
                .WithOne(bp => bp.Bookmark)
                .HasForeignKey(bp => bp.BookmarkId)
                .OnDelete(DeleteBehavior.Cascade);
            // BookmarkPost : One bookmark post has one post
            modelBuilder.Entity<BookmarkPost>()
                .HasOne(bp => bp.Post)
                .WithMany(p => p.BookmarkPosts)
                .HasForeignKey(bp => bp.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            // Share : One share has one post
            modelBuilder.Entity<Share>()
                .HasOne(s => s.Post)
                .WithMany(p => p.Shares)
                .HasForeignKey(s => s.PostId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
