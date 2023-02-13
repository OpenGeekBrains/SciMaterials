using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.ContentTypes;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;

namespace SciMaterials.WebApi.Clients.ContentTypes;

public class ContentTypesClient :
    ApiModifiedClientWithAddBase<Guid, GetContentTypeResponse, AddContentTypeRequest, EditContentTypeRequest>,
    IContentTypesClient
{
    public ContentTypesClient(HttpClient httpClient, ILogger<ContentTypesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.ContentTypes;
}
