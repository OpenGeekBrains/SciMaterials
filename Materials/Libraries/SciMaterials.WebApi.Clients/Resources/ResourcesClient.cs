using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Resources;
using SciMaterials.Contracts.WebApi.Clients.Resources;

namespace SciMaterials.WebApi.Clients.Resources;

public class ResourcesClient :
    ApiReadonlyClientBase<Guid, GetResourceResponse>,
    IResourcesClient
{
    public ResourcesClient(HttpClient httpClient, ILogger<ResourcesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Resources;
}
