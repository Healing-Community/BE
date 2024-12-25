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
    [DbContext(typeof(HFDBPostserviceContext))]
    [Migration("20241225030155_AddShareIdToComment")]
    partial class AddShareIdToComment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Bookmark", b =>
                {
                    b.Property<string>("BookmarkId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("BookmarkId");

                    b.ToTable("Bookmarks");
                });

            modelBuilder.Entity("Domain.Entities.BookmarkPost", b =>
                {
                    b.Property<string>("BookmarkPostId")
                        .HasColumnType("text");

                    b.Property<string>("BookmarkId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("BookmarkPostId");

                    b.HasIndex("BookmarkId");

                    b.HasIndex("PostId", "BookmarkId")
                        .IsUnique();

                    b.ToTable("BookmarkPosts");
                });

            modelBuilder.Entity("Domain.Entities.Category", b =>
                {
                    b.Property<string>("CategoryId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("CategoryId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Domain.Entities.Comment", b =>
                {
                    b.Property<string>("CommentId")
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CoverImgUrl")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ParentId")
                        .HasColumnType("text");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.Property<string>("ShareId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("CommentId");

                    b.HasIndex("ParentId");

                    b.HasIndex("PostId");

                    b.HasIndex("ShareId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Domain.Entities.Post", b =>
                {
                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.Property<string>("CategoryId")
                        .HasColumnType("text");

                    b.Property<string>("CoverImgUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GroupId")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("PostId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Domain.Entities.Reaction", b =>
                {
                    b.Property<string>("ReactionId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.Property<string>("ReactionTypeId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("ReactionId");

                    b.HasIndex("PostId");

                    b.HasIndex("ReactionTypeId");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("Domain.Entities.ReactionType", b =>
                {
                    b.Property<string>("ReactionTypeId")
                        .HasColumnType("text");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ReactionTypeId");

                    b.ToTable("ReactionTypes");
                });

            modelBuilder.Entity("Domain.Entities.Report", b =>
                {
                    b.Property<string>("ReportId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.Property<string>("ReportTypeId")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("ReportId");

                    b.HasIndex("PostId");

                    b.HasIndex("ReportTypeId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Domain.Entities.ReportType", b =>
                {
                    b.Property<string>("ReportTypeId")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ReportTypeId");

                    b.ToTable("ReportTypes");
                });

            modelBuilder.Entity("Domain.Entities.Share", b =>
                {
                    b.Property<string>("ShareId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ShareId");

                    b.HasIndex("PostId");

                    b.ToTable("Shares");
                });

            modelBuilder.Entity("Domain.Entities.UserPreference", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("UserPreferences");
                });

            modelBuilder.Entity("Domain.Entities.BookmarkPost", b =>
                {
                    b.HasOne("Domain.Entities.Bookmark", "Bookmark")
                        .WithMany("BookmarkPosts")
                        .HasForeignKey("BookmarkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Post", "Post")
                        .WithMany("BookmarkPosts")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bookmark");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Domain.Entities.Comment", b =>
                {
                    b.HasOne("Domain.Entities.Comment", "Parent")
                        .WithMany("Replies")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.Entities.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.Entities.Share", "Share")
                        .WithMany()
                        .HasForeignKey("ShareId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Parent");

                    b.Navigation("Post");

                    b.Navigation("Share");
                });

            modelBuilder.Entity("Domain.Entities.Post", b =>
                {
                    b.HasOne("Domain.Entities.Category", "Category")
                        .WithMany("Posts")
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Domain.Entities.Reaction", b =>
                {
                    b.HasOne("Domain.Entities.Post", "Post")
                        .WithMany("Reactions")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.Entities.ReactionType", "ReactionType")
                        .WithMany("Reactions")
                        .HasForeignKey("ReactionTypeId");

                    b.Navigation("Post");

                    b.Navigation("ReactionType");
                });

            modelBuilder.Entity("Domain.Entities.Report", b =>
                {
                    b.HasOne("Domain.Entities.Post", "Post")
                        .WithMany("Reports")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.Entities.ReportType", "ReportType")
                        .WithMany("Reports")
                        .HasForeignKey("ReportTypeId");

                    b.Navigation("Post");

                    b.Navigation("ReportType");
                });

            modelBuilder.Entity("Domain.Entities.Share", b =>
                {
                    b.HasOne("Domain.Entities.Post", "Post")
                        .WithMany("Shares")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Domain.Entities.UserPreference", b =>
                {
                    b.HasOne("Domain.Entities.Category", "Category")
                        .WithMany("UserPreferences")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Domain.Entities.Bookmark", b =>
                {
                    b.Navigation("BookmarkPosts");
                });

            modelBuilder.Entity("Domain.Entities.Category", b =>
                {
                    b.Navigation("Posts");

                    b.Navigation("UserPreferences");
                });

            modelBuilder.Entity("Domain.Entities.Comment", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("Domain.Entities.Post", b =>
                {
                    b.Navigation("BookmarkPosts");

                    b.Navigation("Comments");

                    b.Navigation("Reactions");

                    b.Navigation("Reports");

                    b.Navigation("Shares");
                });

            modelBuilder.Entity("Domain.Entities.ReactionType", b =>
                {
                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("Domain.Entities.ReportType", b =>
                {
                    b.Navigation("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}
