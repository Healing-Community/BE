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
    [DbContext(typeof(ReportDbContext))]
    partial class ReportDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Report", b =>
                {
                    b.Property<Guid>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<Guid>("ExpertId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<string>("ReportTypeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("TargetUserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("ReportId");

                    b.HasIndex("ReportTypeId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Domain.Entities.ReportType", b =>
                {
                    b.Property<string>("ReportTypeId")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("ReportTypeId");

                    b.ToTable("ReportTypes");
                });

            modelBuilder.Entity("Domain.Entities.Report", b =>
                {
                    b.HasOne("Domain.Entities.ReportType", "ReportType")
                        .WithMany("Reports")
                        .HasForeignKey("ReportTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReportType");
                });

            modelBuilder.Entity("Domain.Entities.ReportType", b =>
                {
                    b.Navigation("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}
