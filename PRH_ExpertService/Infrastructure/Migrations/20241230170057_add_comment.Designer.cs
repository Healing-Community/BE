﻿// <auto-generated />
using System;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ExpertDbContext))]
    [Migration("20241230170057_add_comment")]
    partial class add_comment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Appointment", b =>
                {
                    b.Property<string>("AppointmentId")
                        .HasColumnType("text");

                    b.Property<DateOnly>("AppointmentDate")
                        .HasColumnType("date");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("ExpertAvailabilityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExpertEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExpertProfileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MeetLink")
                        .HasColumnType("text");

                    b.Property<int?>("Rating")
                        .HasColumnType("integer");

                    b.Property<string>("RecordingLink")
                        .HasColumnType("text");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time without time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AppointmentId");

                    b.HasIndex("ExpertAvailabilityId");

                    b.HasIndex("ExpertProfileId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("Domain.Entities.Certificate", b =>
                {
                    b.Property<string>("CertificateId")
                        .HasColumnType("text");

                    b.Property<string>("CertificateTypeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ExpertProfileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly?>("ExpirationDate")
                        .HasColumnType("date");

                    b.Property<string>("FileUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly?>("IssueDate")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("VerifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VerifiedByAdminId")
                        .HasColumnType("text");

                    b.HasKey("CertificateId");

                    b.HasIndex("CertificateTypeId");

                    b.HasIndex("ExpertProfileId");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("Domain.Entities.CertificateType", b =>
                {
                    b.Property<string>("CertificateTypeId")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CertificateTypeId");

                    b.ToTable("CertificateTypes");
                });

            modelBuilder.Entity("Domain.Entities.ExpertAvailability", b =>
                {
                    b.Property<string>("ExpertAvailabilityId")
                        .HasColumnType("text");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("AvailableDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("ExpertProfileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time without time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ExpertAvailabilityId");

                    b.HasIndex("ExpertProfileId");

                    b.ToTable("ExpertAvailabilities");
                });

            modelBuilder.Entity("Domain.Entities.ExpertProfile", b =>
                {
                    b.Property<string>("ExpertProfileId")
                        .HasColumnType("text");

                    b.Property<decimal>("AverageRating")
                        .HasColumnType("numeric");

                    b.Property<string>("BackIdCardUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExpertiseAreas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FrontIdCardUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProfileImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Specialization")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ExpertProfileId");

                    b.ToTable("ExpertProfiles");
                });

            modelBuilder.Entity("Domain.Entities.WorkExperience", b =>
                {
                    b.Property<string>("WorkExperienceId")
                        .HasColumnType("text");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("ExpertProfileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PositionTitle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("WorkExperienceId");

                    b.HasIndex("ExpertProfileId");

                    b.ToTable("WorkExperiences");
                });

            modelBuilder.Entity("Domain.Entities.Appointment", b =>
                {
                    b.HasOne("Domain.Entities.ExpertAvailability", "ExpertAvailability")
                        .WithMany("Appointments")
                        .HasForeignKey("ExpertAvailabilityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.ExpertProfile", "ExpertProfile")
                        .WithMany("Appointments")
                        .HasForeignKey("ExpertProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExpertAvailability");

                    b.Navigation("ExpertProfile");
                });

            modelBuilder.Entity("Domain.Entities.Certificate", b =>
                {
                    b.HasOne("Domain.Entities.CertificateType", "CertificateType")
                        .WithMany("Certificates")
                        .HasForeignKey("CertificateTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Entities.ExpertProfile", "ExpertProfile")
                        .WithMany("Certificates")
                        .HasForeignKey("ExpertProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CertificateType");

                    b.Navigation("ExpertProfile");
                });

            modelBuilder.Entity("Domain.Entities.ExpertAvailability", b =>
                {
                    b.HasOne("Domain.Entities.ExpertProfile", "ExpertProfile")
                        .WithMany("ExpertAvailabilities")
                        .HasForeignKey("ExpertProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExpertProfile");
                });

            modelBuilder.Entity("Domain.Entities.WorkExperience", b =>
                {
                    b.HasOne("Domain.Entities.ExpertProfile", "ExpertProfile")
                        .WithMany("WorkExperiences")
                        .HasForeignKey("ExpertProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExpertProfile");
                });

            modelBuilder.Entity("Domain.Entities.CertificateType", b =>
                {
                    b.Navigation("Certificates");
                });

            modelBuilder.Entity("Domain.Entities.ExpertAvailability", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("Domain.Entities.ExpertProfile", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Certificates");

                    b.Navigation("ExpertAvailabilities");

                    b.Navigation("WorkExperiences");
                });
#pragma warning restore 612, 618
        }
    }
}
