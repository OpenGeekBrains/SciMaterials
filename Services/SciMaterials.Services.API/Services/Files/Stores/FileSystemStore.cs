using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.API.Services.Files;

namespace SciMaterials.Services.API.Services.Files.Stores;

public class FileSystemStore : IFileStore
{
    private const long _bufferSize = 100 * 1024 * 1024;
    private readonly ILogger<FileSystemStore> _logger;

    public FileSystemStore(ILogger<FileSystemStore> logger)
    {
        _logger = logger;
    }

    public async Task<FileWriteResult> WriteAsync(string path, string text, CancellationToken Cancel = default)
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        return await WriteAsync(path, memoryStream, Cancel);
    }

    public async Task<FileWriteResult> WriteAsync(string path, Stream stream, CancellationToken Cancel = default)
    {
        try
        {
            CreateDirectoryFromPath(path);
            _logger.LogDebug("Write file {path} started...", path);
            var buffer = new byte[_bufferSize];
            int bytesRead;
            long totalBytesRead = 0;
            using var writeStream = File.Create(path);
            using var hashAlgorithm = SHA512.Create();
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false)) > 0)
            {
                await writeStream.WriteAsync(buffer, 0, bytesRead, Cancel).ConfigureAwait(false);
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
            _logger.LogDebug("The file was saved successfully.");
            return new FileWriteResult(hashString, totalBytesRead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Write file error.");
            throw;
        }
    }

    private void CreateDirectoryFromPath(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(directory))
            throw new DirectoryNotFoundException();

        if (!Directory.Exists(directory))
        {
            _logger.LogDebug("Create directory {directory}", directory);
            Directory.CreateDirectory(directory);
        }
    }

    public Stream OpenRead(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File {Path.GetFileName(path)}  not found");

        var stream = File.OpenRead(path);
        return stream;
    }

    public async Task<T> ReadMetadataAsync<T>(string path, CancellationToken Cancel = default)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Metadata file {Path.GetFileName(path)}  not found");

        using var reader = File.OpenRead(path);
        var metaData = await JsonSerializer.DeserializeAsync<T>(reader).ConfigureAwait(false);

        if (metaData is null)
            throw new NullReferenceException("Metadata cannot be read");

        return metaData;
    }

    public void Delete(string filePath)
    {
        File.Delete(filePath);
    }

    private static string GetMetadataPath(string filePath)
        => Path.ChangeExtension(filePath, "json");
}