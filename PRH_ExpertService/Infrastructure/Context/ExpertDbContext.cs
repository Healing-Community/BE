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

    public DbSet<ExpertProfile> ExpertProfiles { get; set; }
    public DbSet<ExpertAvailability> ExpertAvailabilities { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<CertificateType> CertificateTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.oqidsgyjsueapfermnfj; Password=ProjectHealing@1234");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ExpertProfile Configuration
        modelBuilder.Entity<ExpertProfile>()
            .HasKey(ep => ep.ExpertProfileId);

        modelBuilder.Entity<ExpertProfile>()
            .HasMany(ep => ep.ExpertAvailabilities)
            .WithOne(ea => ea.ExpertProfile)
            .HasForeignKey(ea => ea.ExpertProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExpertProfile>()
            .HasMany(ep => ep.Appointments)
            .WithOne(a => a.ExpertProfile)
            .HasForeignKey(a => a.ExpertProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExpertProfile>()
            .HasMany(ep => ep.WorkExperiences)
            .WithOne(we => we.ExpertProfile)
            .HasForeignKey(we => we.ExpertProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExpertProfile>()
            .HasMany(ep => ep.Certificates)
            .WithOne(c => c.ExpertProfile)
            .HasForeignKey(c => c.ExpertProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // ExpertAvailability Configuration
        modelBuilder.Entity<ExpertAvailability>()
            .HasKey(ea => ea.ExpertAvailabilityId);

        modelBuilder.Entity<ExpertAvailability>()
            .HasMany(ea => ea.Appointments)
            .WithOne(a => a.ExpertAvailability)
            .HasForeignKey(a => a.ExpertAvailabilityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Appointment Configuration
        modelBuilder.Entity<Appointment>()
            .HasKey(a => a.AppointmentId);

        // WorkExperience Configuration
        modelBuilder.Entity<WorkExperience>()
            .HasKey(we => we.WorkExperienceId);

        // Certificate Configuration
        modelBuilder.Entity<Certificate>()
            .HasKey(c => c.CertificateId);

        modelBuilder.Entity<Certificate>()
            .HasOne(c => c.CertificateType)
            .WithMany(ct => ct.Certificates)
            .HasForeignKey(c => c.CertificateTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // CertificateType Configuration
        modelBuilder.Entity<CertificateType>()
            .HasKey(ct => ct.CertificateTypeId);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
