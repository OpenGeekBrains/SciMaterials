using AutoMapper;
using SciMaterials.DAL.Contexts;
using File = SciMaterials.DAL.Models.File;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Contracts.API.Models;
using System.Text.Json;

namespace SciMaterials.Services.API.Services.Files;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IFileStore _fileStore;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _path;
    private readonly string _separator;

    public FileService(
        ILogger<FileService> logger,
        IApiSettings apiSettings,
        IFileStore fileStore,
        IUnitOfWork<SciMaterialsContext> unitOfWork,
        IMapper mapper)
    {
        _logger = logger;
        _fileStore = fileStore;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _path = apiSettings.BasePath;
        _separator = apiSettings.Separator;

        if (string.IsNullOrEmpty(_path))
            throw new ArgumentNullException(nameof(apiSettings.BasePath));
    }

    public async Task<Result<IEnumerable<GetFileResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var files = await _unitOfWork.GetRepository<File>().GetAllAsync();
        var result = _mapper.Map<List<GetFileResponse>>(files);
        return result;
    }

    public async Task<Result<GetFileResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if ((await _unitOfWork.GetRepository<File>().GetByIdAsync(id)) is File file)
        {
            return _mapper.Map<GetFileResponse>(file);
        }

        return Result<GetFileResponse>.Error((int)ResultCodes.NotFound, $"File with ID {id} not found");
    }

    public async Task<Result<GetFileResponse>> GetByHashAsync(string hash)
    {
        if ((await _unitOfWork.GetRepository<File>().GetByHashAsync(hash)) is File file)
        {
            return _mapper.Map<GetFileResponse>(file);
        }

        return await Result<GetFileResponse>.ErrorAsync((int)ResultCodes.NotFound, $"File with hash {hash} not found");
    }

    public async Task<Result<FileStreamInfo>> DownloadByHash(string hash)
    {

        if (await GetByHashAsync(hash) is not { } getFileResponse)
            return Result<FileStreamInfo>.Error((int)ResultCodes.NotFound, $"File with hash {hash} not found");

        var readFromPath = Path.Combine(_path, getFileResponse.Data.Id.ToString());
        return new FileStreamInfo(getFileResponse.Data.Name, getFileResponse.Data.ContentTypeName, _fileStore.OpenRead(readFromPath));
    }


    public async Task<Result<FileStreamInfo>> DownloadById(Guid id)
    {
        if (await GetByIdAsync(id) is not { } getFileResponse)
            return Result<FileStreamInfo>.Error((int)ResultCodes.NotFound, $"File with ID {id} not found");

        var readFromPath = Path.Combine(_path, id.ToString());
        return new FileStreamInfo(getFileResponse.Data.Name, getFileResponse.Data.ContentTypeName, _fileStore.OpenRead(readFromPath));
    }
    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if ((await DeleteFileFromFileSystem(id, cancellationToken)) is { Succeeded: false } deleteFromFileSystemResult)
        {
            return deleteFromFileSystemResult;
        }

        return await DeleteFileFromDatabase(id);
    }

    public async Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default)
    {
        if (await VerifyFileUploadRequest(uploadFileRequest) is { Succeeded: false } verifyFileUploadRequestResult)
        {
            _logger.LogError(verifyFileUploadRequestResult.Messages.First());
            return verifyFileUploadRequestResult;
        }

        var writeToStoreResult = await WriteToStore(uploadFileRequest, fileStream, cancellationToken);
        if (!writeToStoreResult.Succeeded)
            return await Result<Guid>.ErrorAsync(writeToStoreResult.Code, writeToStoreResult.Messages);

        var writeToDatabaseResult = await WriteToDatabase(writeToStoreResult.Data, cancellationToken);
        if (!writeToDatabaseResult.Succeeded)
        {
            _ = DeleteFileFromFileSystem(writeToStoreResult.Data.Id);
            return writeToDatabaseResult;
        }
        return writeToDatabaseResult;
    }
    private async Task<Result<Guid>> VerifyFileUploadRequest(UploadFileRequest uploadFileRequest)
    {
        if (await VerifyContentType(uploadFileRequest.ContentTypeName) is { Succeeded: false } verifyContentTypeResult)
            return await Result<Guid>.ErrorAsync(verifyContentTypeResult.Code, verifyContentTypeResult.Messages);

        if (await VerifyCategories(uploadFileRequest.Categories) is { Succeeded: false } verifyCategoriesResult)
            return await Result<Guid>.ErrorAsync(verifyCategoriesResult.Code, verifyCategoriesResult.Messages);

        if (await _unitOfWork.GetRepository<File>().GetByNameAsync(uploadFileRequest.Name) is { })
            return await Result<Guid>.ErrorAsync((int)ResultCodes.FileAlreadyExist, $"File with name {uploadFileRequest.Name} alredy exist");

        return await Result<Guid>.SuccessAsync();
    }

    private async Task<Result<ContentType>> VerifyContentType(string ContentTypeName)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByNameAsync(ContentTypeName) is { } contentTypeModel)
            return contentTypeModel;

        return await Result<ContentType>.ErrorAsync((int)ResultCodes.NotFound, $"Content type <{ContentTypeName}> not found.");
    }

    private async Task<Result<ICollection<Category>>> VerifyCategories(string categoriesString)
    {
        var categoryIdArray = categoriesString.Split(",").Select(c => Guid.Parse(c)).ToArray();
        var result = new List<Category>(categoryIdArray.Length);
        foreach (var categoryId in categoryIdArray)
        {
            if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId) is not { } category)
                return await Result<ICollection<Category>>.ErrorAsync((int)ResultCodes.NotFound, $"Категория with ID {categoryId} not found");
            result.Add(category);
        }
        return await Result<ICollection<Category>>.SuccessAsync(result);
    }

    private async Task<Result<ICollection<Tag>>> GetTagsFromSeparatedStringAsync(string separator, string tagsSeparatedString)
    {
        var tagsStrings = tagsSeparatedString.Split(separator);
        var result = new List<Tag>(tagsStrings.Length);
        foreach (var tagString in tagsStrings)
        {
            if (await _unitOfWork.GetRepository<Tag>().GetByNameAsync(tagString) is not { } tag)
            {
                tag = new Tag { Name = tagString.ToLower().Trim() };
                await _unitOfWork.GetRepository<Tag>().AddAsync(tag);
            }
            result.Add(tag);
        }

        return result;
    }

    private async Task<Result<Guid>> WriteToDatabase(FileMetadata metadata, CancellationToken cancellationToken = default)
    {
        var verifyContentTypeResult = await VerifyContentType(metadata.ContentTypeName);
        if (!verifyContentTypeResult.Succeeded)
            return await Result<Guid>.ErrorAsync(verifyContentTypeResult.Code, verifyContentTypeResult.Messages);

        var verifyCategoriesResult = await VerifyCategories(metadata.Categories);
        if (!verifyCategoriesResult.Succeeded)
            return await Result<Guid>.ErrorAsync(verifyCategoriesResult.Code, verifyCategoriesResult.Messages);

        // TODO: Change to AspNet Core User                
        var author = (await _unitOfWork.GetRepository<Author>().GetAllAsync()).First();
        if (author is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Author not found.");

        var file = _mapper.Map<File>(metadata);

        file.ContentTypeId = verifyContentTypeResult.Data.Id;
        file.AuthorId = author.Id;

        // TODO: Many to many error... Разобраться
        // file.Categories = verifyCategoriesResult.Data ?? new List<Category>();
        // if (!string.IsNullOrEmpty(metadata.Tags))
        //     file.Tags = (await GetTagsFromSeparatedStringAsync(_separator, metadata.Tags)).Data;

        await _unitOfWork.GetRepository<File>().AddAsync(file);
        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(file.Id, "File created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    private async Task<Result<FileMetadata>> WriteToStore(UploadFileRequest uploadFileRequest, Stream fileStream, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = Guid.NewGuid();
            var path = Path.Combine(_path, id.ToString());
            var fileWriteResult = await _fileStore.WriteAsync(path, fileStream, cancellationToken).ConfigureAwait(false);

            var metadata = _mapper.Map<FileMetadata>(uploadFileRequest);
            metadata.Id = id;
            metadata.Size = fileWriteResult.Size;
            metadata.Hash = fileWriteResult.Hash;

            var metadataJsonString = JsonSerializer.Serialize(metadata);
            _ = await _fileStore.WriteAsync(GetMetadataPath(path), metadataJsonString, cancellationToken).ConfigureAwait(false);

            if (await _unitOfWork.GetRepository<File>().GetByHashAsync(metadata.Hash) is { } existingFile)
            {
                string message = $"File with the same hash {existingFile.Hash} already exists with id: {existingFile.Id.ToString()}";
                _fileStore.Delete(path);
                _logger.LogError(message);
                return await Result<FileMetadata>.ErrorAsync((int)ResultCodes.FileAlreadyExist, message);
            }

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when saving a file to storage");
        }
        return await Result<FileMetadata>.ErrorAsync((int)ResultCodes.ApiError, "Error when saving a file to storage");
    }

    private async Task<Result<Guid>> DeleteFileFromFileSystem(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deletePath = Path.Combine(_path, id.ToString());
            _fileStore.Delete(deletePath);

            return await Result<Guid>.SuccessAsync(id, $"File with ID {id} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when deleting a file  with ID {id} from storage", id);
        }

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, $"Error when deleting a file with ID {id} from storage.");
    }

    private async Task<Result<Guid>> DeleteFileFromDatabase(Guid id, CancellationToken cancellationToken = default)
    {
        var fileRepository = _unitOfWork.GetRepository<File>();

        if (await fileRepository.GetByIdAsync(id) is File file)
        {
            await fileRepository.DeleteAsync(file);
            await _unitOfWork.SaveContextAsync();
        }

        return await Result<Guid>.SuccessAsync($"File with ID {id} deleted");
    }

    private static string GetMetadataPath(string filePath)
        => Path.ChangeExtension(filePath, "json");
}