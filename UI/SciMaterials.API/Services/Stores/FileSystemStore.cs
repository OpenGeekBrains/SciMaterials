using System.Security.Cryptography;
using System.Text.Json;
using SciMaterials.API.Models;
using SciMaterials.API.Services.Interfaces;

namespace SciMaterials.API.Services.Stores;

public class FileSystemStore : IFileStore
{
    private const long _bufferSize = 100 * 1024 * 1024;
    private readonly ILogger<FileSystemStore> _logger;

    public FileSystemStore(ILogger<FileSystemStore> logger)
    {
        _logger = logger;
    }

    public async Task<FileSaveResult> WriteAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[_bufferSize];
        int bytesRead;
        long totalBytesRead = 0;

        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            _logger.LogInformation("Create directory {directory}", directory);
            Directory.CreateDirectory(directory);
        }

        using var writeStream = File.Create(path);
        using var hashAlgorithm = SHA512.Create();
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
        {
            await writeStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
            hashAlgorithm.TransformBlock(buffer, 0, bytesRead, null, 0);
            totalBytesRead += bytesRead;
        }
        hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
        var hash = hashAlgorithm.Hash;
        if (hash is null)
        {
            var exception = new NullReferenceException("Unable to calculate Hash");
            _logger.LogError(exception, null);
            throw exception;
        }

        var hashString = Convert.ToHexString(hash);
        return new FileSaveResult(hashString, totalBytesRead);
    }

    public async Task WriteMetadataAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        var metaData = JsonSerializer.Serialize(data);
        await File.WriteAllTextAsync(path, metaData, cancellationToken).ConfigureAwait(false);
    }

    public Stream OpenRead(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File {Path.GetFileName(path)}  not found");

        var stream = File.OpenRead(path);
        return stream;
    }

    public async Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Metadata file {Path.GetFileName(path)}  not found");

        using var reader = File.OpenRead(path);
        var metaData = await JsonSerializer.DeserializeAsync<T>(reader).ConfigureAwait(false);

        if (metaData is null)
            throw new NullReferenceException("Metadata cannot be read");

        return metaData;
    }

    public void Delete(string path)
    {
        File.Delete(path);
    }
}