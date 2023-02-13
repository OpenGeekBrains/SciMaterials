using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.WebApi.Clients.Categories;

public class CategoriesClient :
    ApiModifiedClientWithAddBase<Guid, GetCategoryResponse, AddCategoryRequest, EditCategoryRequest>,
    ICategoriesClient
{
    public CategoriesClient(HttpClient httpClient, ILogger<CategoriesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Categories;
}
