using SciMaterials.API.Models;

namespace SciMaterials.API.Services.Interfaces;

public interface IFileService
{
    Task<FileModel> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Stream GetFileAsStream(string hash);
}
