using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.Models;

namespace SciMaterials.DAL.Contexts
{
    public partial class EnergoContext : DbContext
    {
        public EnergoContext()
        {
        }

        public EnergoContext(DbContextOptions<EnergoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<ContentType> ContentTypes { get; set; } = null!;
        public virtual DbSet<Models.File> Files { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Text).HasColumnName("text");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comments_owner_id_fk");

                entity.HasMany(d => d.Files)
                    .WithMany(p => p.Comments)
                    .UsingEntity<Dictionary<string, object>>(
                        "CommentsFile",
                        l => l.HasOne<Models.File>()
                            .WithMany()
                            .HasForeignKey("FileId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("comments_files_file_id_fk"),
                        r => r.HasOne<Comment>()
                            .WithMany()
                            .HasForeignKey("CommentId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("comments_files_comment_id_fk"),
                        j =>
                        {
                            j.HasKey("CommentId", "FileId");

                            j.ToTable("comments_files");

                            j.IndexerProperty<Guid>("CommentId").HasColumnName("comment_id");

                            j.IndexerProperty<Guid>("FileId").HasColumnName("file_id");
                        });
            });

            modelBuilder.Entity<ContentType>(entity =>
            {
                entity.ToTable("content_types");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Models.File>(entity =>
            {
                entity.ToTable("files");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.ContentTypeId).HasColumnName("content_type_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("files_category_id_fk");

                entity.HasOne(d => d.ContentType)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.ContentTypeId)
                    .HasConstraintName("files_content_type_id_fk");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("files_owner_id_fk");

                entity.HasMany(d => d.Tags)
                    .WithMany(p => p.Files)
                    .UsingEntity<Dictionary<string, object>>(
                        "FilesTag",
                        l => l.HasOne<Tag>()
                            .WithMany()
                            .HasForeignKey("TagId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("files_tags_tag_id_fk"),
                        r => r.HasOne<Models.File>()
                            .WithMany()
                            .HasForeignKey("FileId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("files_tags_file_id_fk"),
                        j =>
                        {
                            j.HasKey("FileId", "TagId");

                            j.ToTable("files_tags");

                            j.IndexerProperty<Guid>("FileId").HasColumnName("file_id");

                            j.IndexerProperty<Guid>("TagId").HasColumnName("tag_id");
                        });
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tags");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });
        }
    }
}
