﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SciMaterials.DAL.Contexts;

#nullable disable

namespace SciMaterials.SQLiteMigrations.Migrations
{
    [DbContext(typeof(SciMaterialsContext))]
    partial class SciMaterialsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("CategoryResource", b =>
                {
                    b.Property<Guid>("CategoriesId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ResourcesId")
                        .HasColumnType("TEXT");

                    b.HasKey("CategoriesId", "ResourcesId");

                    b.HasIndex("ResourcesId");

                    b.ToTable("CategoryResource");
                });

            modelBuilder.Entity("ResourceTag", b =>
                {
                    b.Property<Guid>("ResourcesId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("TEXT");

                    b.HasKey("ResourcesId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("ResourceTag");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Base.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ResourceType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShortInfo")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ResourceId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ResourceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.ContentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ContentTypes");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Link", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessCount")
                        .IsConcurrencyToken()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastAccess")
                        .IsConcurrencyToken()
                        .HasColumnType("datetime");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("SourceAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Rating", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RatingValue")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ResourceId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.File", b =>
                {
                    b.HasBaseType("SciMaterials.DAL.Models.Base.Resource");

                    b.Property<DateTime?>("AntivirusScanDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("AntivirusScanStatus")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ContentTypeId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("FileGroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortLink")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER");

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("FileGroupId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.FileGroup", b =>
                {
                    b.HasBaseType("SciMaterials.DAL.Models.Base.Resource");

                    b.ToTable("FileGroups");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Url", b =>
                {
                    b.HasBaseType("SciMaterials.DAL.Models.Base.Resource");

                    b.Property<string>("Link")
                        .HasColumnType("TEXT");

                    b.ToTable("Urls");
                });

            modelBuilder.Entity("CategoryResource", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.Base.Resource", null)
                        .WithMany()
                        .HasForeignKey("ResourcesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ResourceTag", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Base.Resource", null)
                        .WithMany()
                        .HasForeignKey("ResourcesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Author", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Base.Resource", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Author", "Author")
                        .WithMany("Resources")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Category", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Category", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Comment", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Author", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.Base.Resource", "Resource")
                        .WithMany("Comments")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Rating", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Author", "User")
                        .WithMany("Ratings")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.Base.Resource", "Resource")
                        .WithMany("Ratings")
                        .HasForeignKey("ResourceId");

                    b.Navigation("Resource");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.File", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.ContentType", "ContentType")
                        .WithMany("Files")
                        .HasForeignKey("ContentTypeId");

                    b.HasOne("SciMaterials.DAL.Models.FileGroup", "FileGroup")
                        .WithMany("Files")
                        .HasForeignKey("FileGroupId");

                    b.HasOne("SciMaterials.DAL.Models.Base.Resource", null)
                        .WithOne()
                        .HasForeignKey("SciMaterials.DAL.Models.File", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentType");

                    b.Navigation("FileGroup");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.FileGroup", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Base.Resource", null)
                        .WithOne()
                        .HasForeignKey("SciMaterials.DAL.Models.FileGroup", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Url", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Base.Resource", null)
                        .WithOne()
                        .HasForeignKey("SciMaterials.DAL.Models.Url", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Author", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Ratings");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Base.Resource", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Ratings");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Category", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.ContentType", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.FileGroup", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
