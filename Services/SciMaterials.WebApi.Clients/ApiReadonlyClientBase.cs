using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.WebApi.Clients;

namespace SciMaterials.WebApi.Clients;

public abstract class ApiReadonlyClientBase<TId, TResult> : ApiClientBase, IApiReadonlyClient<TId, TResult>
{
    public ApiReadonlyClientBase(HttpClient httpClient, ILogger<ApiReadonlyClientBase<TId, TResult>> logger)
        : base(httpClient, logger)
    { }

    public virtual async Task<Result<IEnumerable<TResult>>> GetAllAsync(CancellationToken Cancel = default)
    {
        _logger.LogInformation("Get all entities");

        var result = await _httpClient.GetFromJsonAsync<Result<IEnumerable<TResult>>>(_webApiRoute, Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }

    public virtual async Task<Result<TResult>> GetByIdAsync(TId id, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Get entity by id {id}", id);

        var result = await _httpClient.GetFromJsonAsync<Result<TResult>>($"{_webApiRoute}/{id}", Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }

    public virtual async Task<PageResult<TResult>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Get paged entities >>> Page number:{pageNumber}; Page dize {pageSize}", pageNumber, pageSize);

        var result = await _httpClient.GetFromJsonAsync<PageResult<TResult>>($"{_webApiRoute}/page/{pageNumber}/{pageSize}", Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }
}
