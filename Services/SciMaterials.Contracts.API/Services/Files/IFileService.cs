using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services.Files;

public interface IFileService : IApiService<Guid, GetFileResponse>
{
    Task<Result<GetFileResponse>> GetByIdAsync(Guid id, bool restoreLinks = false, CancellationToken Cancel = default);
    Task<Result<GetFileResponse>> GetByHashAsync(string Hash, bool RestoreLinks = false, CancellationToken Cancel = default);
    Task<Result<Guid>> EditAsync(EditFileRequest EditFileRequest, CancellationToken Cancel = default);
    Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default);
    Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest UploadFileRequest, CancellationToken Cancel = default);
    Task<Result<FileStreamInfo>> DownloadById(Guid id, CancellationToken Cancel = default);
    Task<Result<FileStreamInfo>> DownloadByHash(string hash, CancellationToken Cancel = default);
}
