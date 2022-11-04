using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiAddClient<TId, in TRequest>
{
    Task<Result<TId>> AddAsync(TRequest request, CancellationToken Cancel = default);
}