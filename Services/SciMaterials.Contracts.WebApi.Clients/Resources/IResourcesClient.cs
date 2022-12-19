using SciMaterials.Contracts.API.DTO.Resources;

namespace SciMaterials.Contracts.WebApi.Clients.Resources;

public interface IResourcesClient : IApiReadonlyClient<Guid, GetResourceResponse>
{
}