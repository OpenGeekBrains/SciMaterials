using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.API.Services.Files;

namespace SciMaterials.Services.API.Services.Files.Stores;

public class YandexSore : IFileStore
{
    public void Delete(string path)
    {
        throw new NotImplementedException();
    }

    public Stream OpenRead(string path)
    {
        throw new NotImplementedException();
    }

    public Task<T> ReadMetadataAsync<T>(string path, CancellationToken Cancel = default)
    {
        throw new NotImplementedException();
    }

    public Task<FileWriteResult> WriteAsync(string path, string text, CancellationToken Cancel = default)
    {
        throw new NotImplementedException();
    }

    public Task<FileWriteResult> WriteAsync(string path, Stream stream, CancellationToken Cancel = default)
    {
        throw new NotImplementedException();
    }
}