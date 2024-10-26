using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Context
{
    public partial class HFDBGroupServiceContext : DbContext
    {
        public HFDBGroupServiceContext() { }

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<PostGroup> PostGroups { get; set; }


        public HFDBGroupServiceContext(DbContextOptions<HFDBGroupServiceContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.kulssrgvnfgpytdjvmyy; Password=ProjectHealing@1234");

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB_PostService; Username=postgres; Password=Abcd1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
