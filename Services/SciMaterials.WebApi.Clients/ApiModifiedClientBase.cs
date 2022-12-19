using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.WebApi.Clients;

namespace SciMaterials.WebApi.Clients;

public abstract class ApiModifiedClientBase<TId, TResult, TEditRequest> :
    ApiReadonlyClientBase<TId, TResult>,
    IApiReadonlyClient<TId, TResult>,
    IApiEditClient<TId, TEditRequest>,
    IApiDeleteClient<TId>
{
    public ApiModifiedClientBase(HttpClient httpClient, ILogger<ApiModifiedClientBase<TId, TResult, TEditRequest>> logger)
        : base(httpClient, logger)
    { }

    public virtual async Task<Result<TId>> DeleteAsync(TId id, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Delete entity with id {id}", id);

        var response = await _httpClient.DeleteAsync($"{_webApiRoute}/{id}", Cancel);
        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<Result<TId>>(cancellationToken: Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }

    public virtual async Task<Result<TId>> EditAsync(TEditRequest request, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Edit entity {request}.", request);

        var response = await _httpClient.PutAsJsonAsync($"{_webApiRoute}/Edit", request, Cancel);
        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<Result<TId>>(cancellationToken: Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }
}
