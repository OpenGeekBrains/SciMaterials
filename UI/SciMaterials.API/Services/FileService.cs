using System.Diagnostics;
using System.Security.Cryptography;
using SciMaterials.API.Data.Interfaces;
using SciMaterials.API.Exceptions;
using SciMaterials.API.Models;
using SciMaterials.API.Services.Interfaces;

namespace SciMaterials.API.Services;

public class FileService : IFileService<Guid>
{
    private readonly ILogger<FileService> _logger;
    private readonly IFileRepository<Guid> _fileRepository;
    private readonly IFileStore _fileStore;
    private readonly string _path;
    private readonly bool _overwrite;

    public FileService(ILogger<FileService> logger, IConfiguration configuration, IFileRepository<Guid> fileRepository, IFileStore fileStore)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _fileStore = fileStore;
        _path = configuration.GetValue<string>("BasePath");
        if (string.IsNullOrEmpty(_path))
            throw new ArgumentNullException("Path");
    }

    public FileModel GetFileInfoById(Guid id)
    {
        var model = _fileRepository.GetById(id);

        if (model is null)
            throw new FileNotFoundException($"File with id {id} not found");

        return model;
    }

    public FileModel GetFileInfoByHash(string hash)
    {
        var model = _fileRepository.GetByHash(hash);

        if (model is null)
            throw new FileNotFoundException($"File with hash {hash} not found");

        return model;
    }
    public Stream GetFileStream(Guid id)
    {
        var readFromPath = Path.Combine(_path, id.ToString());
        return _fileStore.OpenRead(readFromPath);
    }

    public async Task<FileModel> UploadAsync(Stream sourceStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var fileNameWithExension = Path.GetFileName(fileName);
        var fileModel = _fileRepository.GetByName(fileNameWithExension);

        if (fileModel is not null && !_overwrite)
        {
            var exception = new FileAlreadyExistException(fileName);
            _logger.LogError(exception, null);
            throw exception;
        }

        var randomFileName = fileModel?.Id ?? Guid.NewGuid();
        var saveToPath = Path.Combine(_path, randomFileName.ToString());
        var metadataPath = Path.Combine(_path, randomFileName + ".json");


        Stopwatch sw = new Stopwatch();
        sw.Start();
        var saveResult = await _fileStore.WriteAsync(saveToPath, sourceStream, cancellationToken).ConfigureAwait(false);
        sw.Stop();
        _logger.LogInformation("Ellapsed:{ellapsed} сек", sw.Elapsed.TotalSeconds);

        if (fileModel is null)
        {
            fileModel = new FileModel
            {
                Id = randomFileName,
                FileName = fileNameWithExension,
                ContentType = contentType,
                Hash = saveResult.Hash,
                Size = saveResult.Size
            };
            _fileRepository.Add(fileModel);
        }
        else
        {
            fileModel.Hash = saveResult.Hash;
            fileModel.Size = saveResult.Size;
            _fileRepository.Update(fileModel);
        }

        await _fileStore.WriteMetadataAsync(metadataPath, fileModel, cancellationToken).ConfigureAwait(false);
        return fileModel;
    }
}
