using SciMaterials.Contracts.API.DTO.Resources;

namespace SciMaterials.Contracts.API.Services.Resources;

public interface IResourceService : IApiService<Guid, GetResourceResponse>
{
}