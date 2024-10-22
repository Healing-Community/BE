using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class ExpertDbContext : DbContext
{
    public ExpertDbContext()
    {
    }

    public ExpertDbContext(DbContextOptions<ExpertDbContext> options) : base(options)
    {
    }

    public virtual DbSet<ExpertProfile> ExpertProfiles { get; set; } = null!;
    public virtual DbSet<Certificate> Certificates { get; set; } = null!;
    public virtual DbSet<CertificateType> CertificateTypes { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.oqidsgyjsueapfermnfj; Password=ProjectHealing@1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ExpertProfile configuration
        modelBuilder.Entity<ExpertProfile>()
            .HasKey(ep => ep.ExpertProfileId);

        modelBuilder.Entity<ExpertProfile>()
            .HasIndex(ep => ep.UserId)
            .IsUnique();

        // CertificateType configuration
        modelBuilder.Entity<CertificateType>()
            .HasKey(ct => ct.CertificateTypeId);

        modelBuilder.Entity<CertificateType>()
            .HasIndex(ct => ct.Name)
            .IsUnique();

        // Certificate configuration
        modelBuilder.Entity<Certificate>()
            .HasKey(c => c.CertificateId);

        modelBuilder.Entity<Certificate>()
            .HasOne(c => c.ExpertProfile)
            .WithMany(ep => ep.Certificates)
            .HasForeignKey(c => c.ExpertProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Certificate>()
            .HasOne(c => c.CertificateType)
            .WithMany(ct => ct.Certificates)
            .HasForeignKey(c => c.CertificateTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
