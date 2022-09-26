using System.Diagnostics;
using System.Security.Cryptography;
using SciMaterials.API.Data.Interfaces;
using SciMaterials.API.Models;
using SciMaterials.API.Services.Interfaces;

namespace SciMaterials.API.Services;

public class FileService : IFileService
{
    private const long _bufferSize = 100 * 1024 * 1024;

    private readonly ILogger<FileService> _logger;
    private readonly IFileRepository _fileRepository;
    private readonly string _path;

    public FileService(ILogger<FileService> logger, IConfiguration configuration, IFileRepository fileRepository)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _path = configuration.GetValue<string>("BasePath");
        if (string.IsNullOrEmpty(_path))
            throw new ArgumentNullException("Path");
    }

    public async Task<FileModel> UploadAsync(Stream sourceStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var fileNameWithExension = Path.GetFileName(fileName);
        var randomFileName = Path.GetRandomFileName();
        var tempPath = Path.GetTempPath();
        var saveToTempPath = Path.Combine(tempPath, randomFileName);

        var fileModel = new FileModel
        {
            FileName = fileNameWithExension,
            RandomFileName = randomFileName,
            ContentType = contentType
        };

        Stopwatch sw = new Stopwatch();
        sw.Start();
        var fileInfo = await SaveTempFile(sourceStream, saveToTempPath, cancellationToken).ConfigureAwait(false);
        sw.Stop();
        _logger.LogInformation("Ellapsed:{ellapsed} сек", sw.Elapsed.TotalSeconds);

        fileModel.Hash = fileInfo.Hash;
        fileModel.Size = fileInfo.Size;

        if (_fileRepository.Add(fileModel))
        {
            MoveToStore(saveToTempPath);
        }
        else
        {
            File.Delete(saveToTempPath);
            fileModel = _fileRepository.GetByHash(fileModel.Hash);
        }

        return fileModel;
    }

    public Stream GetFileAsStream(string hash)
    {
        var model = _fileRepository.GetByHash(hash);
        if (model is null)
            throw new FileNotFoundException($"File with hash {hash} not found");

        var readFromPath = Path.Combine(_path, model.RandomFileName);
        if (!File.Exists(readFromPath))
            throw new FileNotFoundException($"File with hash {hash} not found");

        return File.OpenRead(readFromPath);

    }

    private async Task<(string Hash, long Size)> SaveTempFile(Stream sourceStream, string saveToPath, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[_bufferSize];
        int bytesRead;
        long totalBytesRead = 0;

        using var targetStream = File.Create(saveToPath);
        using var hashAlgorithm = SHA512.Create();
        while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
        {
            await targetStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
            hashAlgorithm.TransformBlock(buffer, 0, bytesRead, null, 0);
            totalBytesRead += bytesRead;
        }

        hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
        var hash = Convert.ToHexString(hashAlgorithm.Hash);
        return (hash, totalBytesRead);
    }

    private void MoveToStore(string sourceFileName)
    {
        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        var saveToPath = Path.Combine(_path, Path.GetFileName(sourceFileName));
        File.Move(sourceFileName, saveToPath);
    }
}