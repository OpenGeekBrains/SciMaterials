using Microsoft.Extensions.Logging;

namespace SciMaterials.WebApi.Clients;

public abstract class ApiClientBase
{
    protected readonly ILogger<ApiClientBase> _logger;
    protected readonly HttpClient _httpClient;
    protected string _webApiRoute = string.Empty;

    public ApiClientBase(HttpClient httpClient, ILogger<ApiClientBase> logger)
        => (_httpClient, _logger) = (httpClient, logger);

}
