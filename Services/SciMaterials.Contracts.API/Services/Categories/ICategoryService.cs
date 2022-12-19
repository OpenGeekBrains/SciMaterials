using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services.Categories;

public interface ICategoryService : IApiService<Guid, GetCategoryResponse>, IModifyService<AddCategoryRequest, EditCategoryRequest, Guid>
{
    Task<Result<CategoryTree>> GetTreeAsync(Guid? id, CancellationToken Cancel = default);
}