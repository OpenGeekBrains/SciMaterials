using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.WebApi.Clients;

namespace SciMaterials.WebApi.Clients;

public abstract class ApiModifiedClientWithAddBase<TId, TResult, TAddRequest, TEditRequest> :
    ApiModifiedClientBase<TId, TResult, TEditRequest>,
    IApiReadonlyClient<TId, TResult>,
    IApiModifyClient<TId, TAddRequest, TEditRequest>
{
    public ApiModifiedClientWithAddBase(HttpClient httpClient, ILogger<ApiModifiedClientWithAddBase<TId, TResult, TAddRequest, TEditRequest>> logger)
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
