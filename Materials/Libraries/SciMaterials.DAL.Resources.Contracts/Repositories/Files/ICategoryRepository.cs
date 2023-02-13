using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.DAL.Resources.Contracts.Repositories.Files;

/// <summary> Интерфейс репозитория для <see cref="Category"/>. </summary>
public interface ICategoryRepository : IRepository<Category>
{
    public Task<IEnumerable<Category>> GetByParentIdAsync(Guid? parentId);
}