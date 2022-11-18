using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.Contracts.WebApi.Clients.Urls;

namespace SciMaterials.WebApi.Clients.Urls;

public class UrlsClient :
    ApiClientWithAddBase<Guid, GetUrlResponse, AddUrlRequest, EditUrlRequest>,
    IUrlsClient
{
    public UrlsClient(HttpClient httpClient, ILogger<UrlsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Urls;
}
