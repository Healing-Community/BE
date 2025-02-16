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
    [DbContext(typeof(HFDBNotificationServiceContext))]
    partial class HFDBNotificationServiceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Notification", b =>
                {
                    b.Property<string>("NotificationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NotificationTypeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("NotificationId");

                    b.HasIndex("NotificationTypeId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Domain.Entities.NotificationType", b =>
                {
                    b.Property<string>("NotificationTypeId")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("NotificationTypeId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("NotificationTypes");
                });

            modelBuilder.Entity("Domain.Entities.UserNotificationPreference", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("NotificationTypeId")
                        .HasColumnType("text");

                    b.Property<bool>("IsSubscribed")
                        .HasColumnType("boolean");

                    b.HasKey("UserId", "NotificationTypeId");

                    b.HasIndex("NotificationTypeId");

                    b.ToTable("UserNotificationPreferences");
                });

            modelBuilder.Entity("Domain.Entities.Notification", b =>
                {
                    b.HasOne("Domain.Entities.NotificationType", "NotificationType")
                        .WithMany("Notifications")
                        .HasForeignKey("NotificationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NotificationType");
                });

            modelBuilder.Entity("Domain.Entities.UserNotificationPreference", b =>
                {
                    b.HasOne("Domain.Entities.NotificationType", "NotificationType")
                        .WithMany("UserPreferences")
                        .HasForeignKey("NotificationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NotificationType");
                });

            modelBuilder.Entity("Domain.Entities.NotificationType", b =>
                {
                    b.Navigation("Notifications");

                    b.Navigation("UserPreferences");
                });
#pragma warning restore 612, 618
        }
    }
}
