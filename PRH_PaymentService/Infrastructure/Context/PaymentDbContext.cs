using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class PaymentDbContext : DbContext
{
    public PaymentDbContext()
    {
    }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.xdqkzkedyxzuajhxerjt; Password=ProjectHealing@1234");


    public virtual DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PRIMARY KEY
        modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);

        // Define foreign keys (logical relationship, without physical database constraint)
        modelBuilder.Entity<Payment>()
            .Property(p => p.UserId)
            .IsRequired();

        modelBuilder.Entity<Payment>()
            .Property(p => p.AppointmentId)
            .IsRequired();

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
