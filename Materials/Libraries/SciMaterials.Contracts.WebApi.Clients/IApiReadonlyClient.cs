using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiReadonlyClient<in TId, TResult>
{
    Task<Result<IEnumerable<TResult>>> GetAllAsync(CancellationToken Cancel = default);
    Task<Result<TResult>> GetByIdAsync(TId id, CancellationToken Cancel = default);
    Task<PageResult<TResult>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default);
}