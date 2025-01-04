﻿// <auto-generated />
using System;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(PaymentDbContext))]
    partial class PaymentDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Payment", b =>
                {
                    b.Property<string>("PaymentId")
                        .HasColumnType("text");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<string>("AppointmentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExpertPaymentQrCodeLink")
                        .HasColumnType("text");

                    b.Property<long>("OrderCode")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PaymentDetail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserPaymentQrCodeLink")
                        .HasColumnType("text");

                    b.HasKey("PaymentId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Domain.Entities.PlatformFee", b =>
                {
                    b.Property<string>("PlatformFeeId")
                        .HasColumnType("text");

                    b.Property<string>("PlatformFeeDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlatformFeeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlatformFeeValue")
                        .HasColumnType("integer");

                    b.HasKey("PlatformFeeId");

                    b.ToTable("PlatformFees");
                });
#pragma warning restore 612, 618
        }
    }
}
