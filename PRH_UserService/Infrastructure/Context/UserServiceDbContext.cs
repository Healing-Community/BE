﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class UserServiceDbContext : DbContext
{
    public UserServiceDbContext()
    {
    }

    public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : base(options)
    {
    }
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Role> Roles { get; set; } = null!;
    public virtual DbSet<Token> Tokens { get; set; } = null!;
    public virtual DbSet<Follower> Followers { get; set; } = null!;
    public virtual DbSet<SocialLink> SocialLinks { get; set; } = null!;
    public virtual DbSet<PaymentInfo> PaymentInfos { get; set; } = null!;

    // this method when production because it will load from the configuration file
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com; Database=postgres; Username=postgres.cggerynfjmvyretpnrzy; Password=ProjectHealing@1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User configuration
        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId); // Assuming UserId is Guid or string
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .Property(u => u.UserName)
            .IsRequired(false);
        modelBuilder.Entity<User>()
            .Property(u => u.FullName)
            .IsRequired(false);
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Role configuration
        modelBuilder.Entity<Role>()
            .HasKey(r => r.RoleId);
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.RoleName)
            .IsUnique();

        // Token configuration
        modelBuilder.Entity<Token>()
            .HasKey(t => t.TokenId); // Use TokenId as primary key
        modelBuilder.Entity<Token>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Token>()
            .HasIndex(t => t.RefreshToken)
            .IsUnique();

        // SocialLink configuration
        modelBuilder.Entity<SocialLink>()
            .HasKey(sl => sl.LinkId);
        modelBuilder.Entity<SocialLink>()
            .HasOne(sl => sl.User)
            .WithMany(u => u.SocialLinks)
            .HasForeignKey(sl => sl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Follower configuration
        modelBuilder.Entity<Follower>()
            .HasKey(f => f.Id);
        modelBuilder.Entity<Follower>()
            .HasOne(f => f.User)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.UserId)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Follower>() // Add unique constraint to prevent duplicate follows
            .HasIndex(f => new { f.FollowerId, f.UserId })
            .IsUnique();
        // PaymentInfo configuration
        modelBuilder.Entity<PaymentInfo>()
            .HasKey(pi => pi.PaymentInfoId);
        modelBuilder.Entity<PaymentInfo>()
            .HasOne(pi => pi.User)
            .WithOne(u => u.PaymentInfo)
            .HasForeignKey<PaymentInfo>(pi => pi.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Other configurations
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}