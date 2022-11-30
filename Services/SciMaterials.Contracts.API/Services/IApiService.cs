using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services;

public interface IApiService<in TId, TResult>
{
    Task<Result<IEnumerable<TResult>>> GetAllAsync(CancellationToken Cancel = default);
    Task<PageResult<TResult>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default);
    Task<Result<TResult>> GetByIdAsync(TId id, CancellationToken Cancel = default);
}
