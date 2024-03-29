﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SciMaterials.DAL.Resources.Contexts;

#nullable disable

namespace SciMaterials.Materials.DAL.SqlServer.Migrations.Migrations
{
    [DbContext(typeof(SciMaterialsContext))]
    [Migration("20230618153056_RemoveLinks")]
    partial class RemoveLinks
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CategoryResource", b =>
                {
                    b.Property<Guid>("CategoriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ResourcesId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CategoriesId", "ResourcesId");

                    b.HasIndex("ResourcesId");

                    b.ToTable("CategoryResource");
                });

            modelBuilder.Entity("ResourceTag", b =>
                {
                    b.Property<Guid>("ResourcesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ResourcesId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("ResourceTag");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.ContentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ContentTypes");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Rating", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("RatingValue")
                        .HasColumnType("int");

                    b.Property<Guid?>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortInfo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Resources");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.File", b =>
                {
                    b.HasBaseType("SciMaterials.DAL.Resources.Contracts.Entities.Resource");

                    b.Property<DateTime?>("AntivirusScanDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("AntivirusScanStatus")
                        .HasColumnType("int");

                    b.Property<Guid?>("ContentTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("FileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Hash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("FileGroupId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.FileGroup", b =>
                {
                    b.HasBaseType("SciMaterials.DAL.Resources.Contracts.Entities.Resource");

                    b.ToTable("FileGroups");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Url", b =>
                {
                    b.HasBaseType("SciMaterials.DAL.Resources.Contracts.Entities.Resource");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Urls");
                });

            modelBuilder.Entity("CategoryResource", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Resource", null)
                        .WithMany()
                        .HasForeignKey("ResourcesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ResourceTag", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Resource", null)
                        .WithMany()
                        .HasForeignKey("ResourcesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Author", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Category", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Category", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Comment", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Author", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Resource", "Resource")
                        .WithMany("Comments")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Rating", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Author", "User")
                        .WithMany("Ratings")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Resource", "Resource")
                        .WithMany("Ratings")
                        .HasForeignKey("ResourceId");

                    b.Navigation("Resource");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Resource", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Author", "Author")
                        .WithMany("Resources")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.File", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.ContentType", "ContentType")
                        .WithMany("Files")
                        .HasForeignKey("ContentTypeId");

                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.FileGroup", "FileGroup")
                        .WithMany("Files")
                        .HasForeignKey("FileGroupId");

                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Resource", null)
                        .WithOne()
                        .HasForeignKey("SciMaterials.DAL.Resources.Contracts.Entities.File", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentType");

                    b.Navigation("FileGroup");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.FileGroup", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Resource", null)
                        .WithOne()
                        .HasForeignKey("SciMaterials.DAL.Resources.Contracts.Entities.FileGroup", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Url", b =>
                {
                    b.HasOne("SciMaterials.DAL.Resources.Contracts.Entities.Resource", null)
                        .WithOne()
                        .HasForeignKey("SciMaterials.DAL.Resources.Contracts.Entities.Url", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Author", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Ratings");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Category", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.ContentType", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.Resource", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Ratings");
                });

            modelBuilder.Entity("SciMaterials.DAL.Resources.Contracts.Entities.FileGroup", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
