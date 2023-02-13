using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiEditClient<TId, in TRequest>
{
    Task<Result<TId>> EditAsync(TRequest request, CancellationToken Cancel = default);
}