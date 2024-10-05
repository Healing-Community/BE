using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public partial class HFDbContext : DbContext
    {
        public HFDbContext() { }

        public HFDbContext(DbContextOptions<HFDbContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost; Database=HFDB; Username=postgres; Password=Abcd1234");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
