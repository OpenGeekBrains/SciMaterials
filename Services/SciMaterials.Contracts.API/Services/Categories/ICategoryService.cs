using SciMaterials.Contracts.API.DTO.Categories;

namespace SciMaterials.Contracts.API.Services.Categories;

public interface ICategoryService : IApiService<Guid, GetCategoryResponse>, IModifyService<AddCategoryRequest, EditCategoryRequest, Guid>
{
}