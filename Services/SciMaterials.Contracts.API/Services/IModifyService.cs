using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services;

public interface IModifyService<in TAddModel, in TEditModel, TId>
    where TAddModel : class
    where TEditModel : class
{
    Task<Result<TId>> AddAsync(TAddModel model, CancellationToken Cancel = default);
    Task<Result<TId>> EditAsync(TEditModel model, CancellationToken Cancel = default);
    Task<Result<TId>> DeleteAsync(TId id, CancellationToken Cancel = default);
}
