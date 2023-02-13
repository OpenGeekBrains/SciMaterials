using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary> Репозиторий для <see cref="Tag"/>. </summary>
public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(SciMaterialsContext context, ILogger<TagRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<Tag> GetIncludeQuery(IQueryable<Tag> query) => query
       .Include(t => t.Resources);

    protected override Tag UpdateCurrentEntity(Tag DataEntity, Tag DbEntity)
    {
        DbEntity.Resources = DataEntity.Resources;
        DbEntity.Name = DataEntity.Name;
        DbEntity.IsDeleted = DataEntity.IsDeleted;

        return DbEntity;
    }
}