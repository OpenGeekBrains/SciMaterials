using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.WebApi.Clients;

using System.Net.Http.Json;

namespace SciMaterials.WebApi.Clients;

public abstract class ApiClientWithAddBase<TId, TResult, TAddRequest, TEditRequest> :
    ApiClientBase<TId, TResult, TEditRequest>,
    IApiModifyClient<TId, TAddRequest, TEditRequest>
{
    public ApiClientWithAddBase(HttpClient httpClient, ILogger<ApiClientWithAddBase<TId, TResult, TAddRequest, TEditRequest>> logger) 
        : base(httpClient, logger)
    { }

    public virtual async Task<Result<TId>> AddAsync(TAddRequest request, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Add entity {request}", request);

        var response = await _httpClient.PutAsJsonAsync($"{_webApiRoute}/Add", request, Cancel);
        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<Result<TId>>(cancellationToken: Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }
}
