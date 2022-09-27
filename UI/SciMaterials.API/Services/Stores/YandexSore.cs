using System.Security.Cryptography;
using System.Text.Json;
using SciMaterials.API.Models;
using SciMaterials.API.Services.Interfaces;

namespace SciMaterials.API.Services.Stores;

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

    public Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<FileSaveResult> WriteAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task WriteMetadataAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}