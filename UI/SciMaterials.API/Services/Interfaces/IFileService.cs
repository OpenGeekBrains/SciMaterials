using SciMaterials.API.Models;

namespace SciMaterials.API.Services.Interfaces;

public interface IFileService<T>
{
    Task<FileModel> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Stream GetFileStream(T id);
    FileModel GetFileInfoById(T id);
    FileModel GetFileInfoByHash(string hash);
}
