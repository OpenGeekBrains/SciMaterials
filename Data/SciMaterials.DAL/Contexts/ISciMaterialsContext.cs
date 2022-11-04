using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using SciMaterials.DAL.Models;

namespace SciMaterials.DAL.Contexts;

public interface ISciMaterialsContext
{
    DbSet<Category> Categories { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<ContentType> ContentTypes { get; set; }
    DbSet<FileGroup> FileGroups { get; set; }
    DbSet<Models.File> Files { get; set; }
    DbSet<Url> Urls { get; set; }
    DbSet<Rating> Ratings { get; set; }
    DbSet<Tag> Tags { get; set; }
    DbSet<Author> Authors { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<T> Set<T>() where T : class;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

}