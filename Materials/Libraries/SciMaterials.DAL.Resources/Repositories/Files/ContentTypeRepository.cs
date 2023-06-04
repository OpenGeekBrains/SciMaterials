using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary> Репозиторий для <see cref="ContentType"/>. </summary>
public class ContentTypeRepository : Repository<ContentType>, IContentTypeRepository
{
    public ContentTypeRepository(SciMaterialsContext context, ILogger<ContentTypeRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<ContentType> GetIncludeQuery(IQueryable<ContentType> query) => query
       .Include(t => t.Files);

    public override ContentType? GetByName(string Name)
    {
        var items = ItemsNotDeleted.FirstOrDefault(item => item.Name == Name);
        return items;
    }

    public override async Task<ContentType?> GetByNameAsync(string Name)
    {
        var items = await ItemsNotDeleted.FirstOrDefaultAsync(item => item.Name == Name);
        return items;
    }

    protected override ContentType UpdateCurrentEntity(ContentType DataEntity, ContentType DbEntity)
    {
        DbEntity.Files = DataEntity.Files;
        DbEntity.Name = DataEntity.Name;
        DbEntity.FileExtension = DataEntity.FileExtension;
        DbEntity.IsDeleted = DataEntity.IsDeleted;

        return DbEntity;
    }
}