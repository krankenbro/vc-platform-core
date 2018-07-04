﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.ImageToolsModule.Data.Repositories;

namespace VirtoCommerce.ImageToolsModule.Data.Migrations
{
    [DbContext(typeof(ThumbnailDbContext))]
    [Migration("20180621151815_Thumbnail")]
    partial class Thumbnail
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.ImageToolsModule.Data.Models.ThumbnailOptionEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("AnchorPosition")
                        .HasMaxLength(64);

                    b.Property<string>("BackgroundColor");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("FileSuffix")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<int?>("Height");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1024);

                    b.Property<string>("ResizeMethod")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<int?>("Width");

                    b.HasKey("Id");

                    b.ToTable("ThumbnailOption");
                });

            modelBuilder.Entity("VirtoCommerce.ImageToolsModule.Data.Models.ThumbnailTaskEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime?>("LastRun");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1024);

                    b.Property<string>("WorkPath")
                        .IsRequired()
                        .HasMaxLength(2048);

                    b.HasKey("Id");

                    b.ToTable("ThumbnailTask");
                });

            modelBuilder.Entity("VirtoCommerce.ImageToolsModule.Data.Models.ThumbnailTaskOptionEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ThumbnailOptionId");

                    b.Property<string>("ThumbnailTaskId");

                    b.HasKey("Id");

                    b.HasIndex("ThumbnailOptionId");

                    b.HasIndex("ThumbnailTaskId");

                    b.ToTable("ThumbnailTaskOption");
                });

            modelBuilder.Entity("VirtoCommerce.ImageToolsModule.Data.Models.ThumbnailTaskOptionEntity", b =>
                {
                    b.HasOne("VirtoCommerce.ImageToolsModule.Data.Models.ThumbnailOptionEntity", "ThumbnailOption")
                        .WithMany()
                        .HasForeignKey("ThumbnailOptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VirtoCommerce.ImageToolsModule.Data.Models.ThumbnailTaskEntity", "ThumbnailTask")
                        .WithMany("ThumbnailTaskOptions")
                        .HasForeignKey("ThumbnailTaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
