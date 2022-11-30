using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary> Репозиторий для <see cref="FileGroup"/>. </summary>
public class FileGroupRepository : Repository<FileGroup>, IFileGroupRepository
{
    public FileGroupRepository(SciMaterialsContext context, ILogger<FileGroupRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<FileGroup> GetIncludeQuery(IQueryable<FileGroup> query) => query
       .Include(fg => fg.Files)
       .Include(fg => fg.Tags)
       .Include(fg => fg.Ratings)
       .Include(fg => fg.Comments)
       .Include(fg => fg.Categories)
       .Include(fg => fg.Author);

    protected override FileGroup UpdateCurrentEntity(FileGroup DataEntity, FileGroup DbEntity)
    {
        DbEntity.Name = DataEntity.Name;
        DbEntity.IsDeleted = DataEntity.IsDeleted;

        DbEntity.ShortInfo = DataEntity.ShortInfo;
        DbEntity.Description = DataEntity.Description;
        DbEntity.AuthorId = DataEntity.AuthorId;
        DbEntity.CreatedAt = DataEntity.CreatedAt;
        DbEntity.Author = DataEntity.Author;
        DbEntity.Comments = DataEntity.Comments;
        DbEntity.Tags = DataEntity.Tags;
        DbEntity.Categories = DataEntity.Categories;
        DbEntity.Ratings = DataEntity.Ratings;

        DbEntity.Files = DataEntity.Files;

        return DbEntity;
    }
}