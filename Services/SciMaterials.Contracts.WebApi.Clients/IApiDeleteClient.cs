using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiDeleteClient<TId>
{
    Task<Result<TId>> DeleteAsync(TId id, CancellationToken Cancel = default);
}