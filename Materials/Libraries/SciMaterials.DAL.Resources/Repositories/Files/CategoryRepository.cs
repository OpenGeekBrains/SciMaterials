using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary>Репозиторий для <see cref="Category"/></summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(SciMaterialsContext Context, ILogger<CategoryRepository> Logger) : base(Context, Logger) { }

    public override async Task<Category?> GetByNameAsync(string Name)
    {
        var items = await ItemsNotDeleted.FirstOrDefaultAsync(c => c.Name == Name && !c.IsDeleted);
        return items;
    }

    public override Category? GetByName(string Name)
    {
        var items = ItemsNotDeleted.FirstOrDefault(c => c.Name == Name);
        return items;
    }

    public override Category? GetByHash(string Hash) => null;

    protected override Category UpdateCurrentEntity(Category DataEntity, Category DbEntity)
    {
        DbEntity.Description = DataEntity.Description;
        DbEntity.CreatedAt = DataEntity.CreatedAt;
        DbEntity.Resources = DataEntity.Resources; ;
        DbEntity.ParentId = DataEntity.ParentId;
        DbEntity.Name = DataEntity.Name;
        DbEntity.IsDeleted = DataEntity.IsDeleted;

        return DbEntity;
    }

    public async Task<IEnumerable<Category>> GetByParentIdAsync(Guid? ParentId)
    {
        var query = _Set
            .Where(C => C.ParentId == ParentId && !C.IsDeleted)
            .Include(C => C.Children)
            .AsNoTracking();

        var items = await ItemsNotDeleted.ToListAsync();
        return items;
    }
}