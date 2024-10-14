using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class ReportDbContext : DbContext
{
    public ReportDbContext()
    {
    }

    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Report> Reports { get; set; }
    public virtual DbSet<ReportType> ReportTypes { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.cggerynfjmvyretpnrzy; Password=ProjectHealing@1234");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost; Database=ReportService; Username=postgres; Password=Abcd1234");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PRIMARY KEY
        modelBuilder.Entity<Report>().HasKey(r => r.ReportId);
        modelBuilder.Entity<ReportType>().HasKey(rt => rt.ReportTypeId);
        // Relationship
        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReportType)
            .WithMany(rt => rt.Reports)
            .HasForeignKey(r => r.ReportTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}