using AutoMapper;
using SciMaterials.DAL.Contexts;
using File = SciMaterials.DAL.Models.File;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Contracts.API.Models;
using System.Text.Json;
using SciMaterials.Contracts;
using SciMaterials.Contracts.ShortLinks;
using System.Reflection.Metadata.Ecma335;

namespace SciMaterials.Services.API.Services.Files;

public class FileService : ApiServiceBase, IFileService
{
    private readonly IFileStore _fileStore;
    private readonly ILinkReplaceService _linkReplaceService;
    private readonly ILinkShortCutService _linkShortCutService;
    private readonly string _path;
    private readonly string _separator;

    public FileService(
        IApiSettings apiSettings,
        IFileStore fileStore,
        ILinkReplaceService linkReplaceService,
        ILinkShortCutService linkShortCutService,
        IUnitOfWork<SciMaterialsContext> unitOfWork,
        IMapper mapper,
        ILogger<FileService> logger) : base(unitOfWork, mapper, logger)
    {
        _fileStore = fileStore;
        _linkReplaceService = linkReplaceService;
        _linkShortCutService = linkShortCutService;
        _path = apiSettings.BasePath;
        _separator = apiSettings.Separator;

        if (string.IsNullOrEmpty(_path))
            throw new ArgumentNullException(nameof(apiSettings.BasePath));
    }

    public async Task<Result<IEnumerable<GetFileResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var files = await _unitOfWork.GetRepository<File>().GetAllAsync(include: true);
        var result = _mapper.Map<List<GetFileResponse>>(files);
        return result;
    }

    public async Task<PageResult<GetFileResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var files = await _unitOfWork.GetRepository<File>().GetPageAsync(pageNumber, pageSize, include: true);
        var totalCount = await _unitOfWork.GetRepository<File>().GetCountAsync();
        var result = _mapper.Map<List<GetFileResponse>>(files);
        return (result, totalCount);
    }

    public async Task<Result<GetFileResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        var repository = _unitOfWork.GetRepository<File>();

        if (await repository.GetByIdAsync(id, include: true) is not { } file)
        {
            return LoggedError<GetFileResponse>(
                Errors.Api.File.NotFound,
                "File with ID {id} not found",
                id);
        }

        var result = _mapper.Map<GetFileResponse>(file);
        return result;
    }

    public async Task<Result<GetFileResponse>> GetByIdAsync(Guid id, bool restoreLinks = false, CancellationToken Cancel = default)
    {
        var result = await GetByIdAsync(id, Cancel);

        if (restoreLinks && result.Succeeded && result.Data?.Description is { Length: > 0 })
        {
            var description = await _linkReplaceService.RestoreLinksAsync(result.Data.Description, Cancel).ConfigureAwait(false);
            result.Data.Description = description;
        }

        return result;
    }

    public async Task<Result<GetFileResponse>> GetByHashAsync(string hash, bool restoreLinks = false, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<File>().GetByHashAsync(hash, include: true) is not { } file)
        {
            return LoggedError<GetFileResponse>(
                Errors.Api.File.NotFound,
                "File with hash {hash} not found",
                hash);
        }

        var result = _mapper.Map<GetFileResponse>(file);

        if (restoreLinks && result.Description is { Length: > 0 })
        {
            var description = await _linkReplaceService.RestoreLinksAsync(result.Description, Cancel).ConfigureAwait(false);
            result.Description = description;
        }

        return result;
    }

    public async Task<Result<Guid>> EditAsync(EditFileRequest editFileRequest, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<File>().GetByIdAsync(editFileRequest.Id, false, true) is not { } existingFile)
        {
            return LoggedError<Guid>(
                Errors.Api.File.NotFound,
                "File with ID {fileId} not found",
                editFileRequest.Id);
        }

        var verifyCategoriesResult = await VerifyCategories(_separator, editFileRequest.Categories);
        if (!verifyCategoriesResult.Succeeded)
        {
            return Result<Guid>.Failure(verifyCategoriesResult);
        }

        var file = _mapper.Map(editFileRequest, existingFile);

        if (verifyCategoriesResult.Data is { })
        {
            file.Categories.Clear();
            file.Categories = verifyCategoriesResult.Data;
        }

        if (editFileRequest.Tags is { Length: > 0 })
        {
            var getTagsResult = await GetTagsFromSeparatedStringAsync(_separator, editFileRequest.Tags);
            file.Tags?.Clear();
            file.Tags = getTagsResult.Data;
        }

        if (file.Description is { Length: > 0 })
        {
            var description = await _linkReplaceService.ShortenLinksAsync(file.Description, Cancel).ConfigureAwait(false);
            file.Description = description;
        }

        await _unitOfWork.GetRepository<File>().UpdateAsync(file);
        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.File.Update,
                "File {fileName} update error",
                editFileRequest.Name);
        }

        _logger.LogInformation("File {fileName} updated.", editFileRequest.Name);
        return file.Id;
    }

    public async Task<Result<FileStreamInfo>> DownloadByHash(string hash, CancellationToken Cancel = default)
    {

        if (await GetByHashAsync(hash, false, Cancel) is not { } getFileResponse)
        {
            return LoggedError<FileStreamInfo>(
                Errors.Api.File.NotFound,
                "File with hash {fileHash} not found",
                hash);
        }

        var readFromPath = Path.Combine(_path, getFileResponse.Data.Id.ToString());
        try
        {
            var fileStream = _fileStore.OpenRead(readFromPath);
            return new FileStreamInfo(getFileResponse.Data.Name, getFileResponse.Data.ContentTypeName, fileStream);
        }
        catch (Exception ex)
        {
            return LoggedError<FileStreamInfo>(Errors.Api.File.Download, ex, "Download by hash {fileHash} error", hash);
        }
    }

    public async Task<Result<FileStreamInfo>> DownloadById(Guid id, CancellationToken Cancel = default)
    {
        if (await GetByIdAsync(id, Cancel) is not { } getFileResponse)
        {
            return LoggedError<FileStreamInfo>(
                Errors.Api.File.NotFound,
                "File with ID {fileId} not found",
                id);
        }

        var readFromPath = Path.Combine(_path, id.ToString());
        try
        {
            var fileStream = _fileStore.OpenRead(readFromPath);
            return new FileStreamInfo(getFileResponse.Data.Name, getFileResponse.Data.ContentTypeName, fileStream);
        }
        catch (Exception ex)
        {
            return LoggedError<FileStreamInfo>(
                Errors.Api.File.Download,
                ex,
                "Download by ID {fileId} error",
                id);
        }
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await DeleteFileFromFileSystem(id, Cancel) is { Succeeded: false } deleteFromFileSystemResult)
        {
            return Result<Guid>.Failure(deleteFromFileSystemResult);
        }

        return await DeleteFileFromDatabase(id);
    }

    public async Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken Cancel = default)
    {
        if (await VerifyFileUploadRequest(uploadFileRequest) is { Succeeded: false } verifyFileUploadRequestResult)
        {
            return Result<Guid>.Failure(verifyFileUploadRequestResult);
        }

        if (uploadFileRequest.Description is { Length: > 0 })
        {
            var descriptionWithShortLinks = await _linkReplaceService.ShortenLinksAsync(uploadFileRequest.Description, Cancel);
            uploadFileRequest.Description = descriptionWithShortLinks;
        }

        var writeToStoreResult = await WriteFileToStoreAsync(uploadFileRequest, fileStream, Cancel);
        if (!writeToStoreResult.Succeeded)
        {
            return Result<Guid>.Failure(writeToStoreResult);
        }

        var writeMetadataToStoreResult = await WriteMetadataToStoreAsync(writeToStoreResult.Data, Cancel);
        if (!writeMetadataToStoreResult.Succeeded)
        {
            return Result<Guid>.Failure(writeMetadataToStoreResult);
        }

        var writeToDatabaseResult = await WriteToDatabaseAsync(writeToStoreResult.Data, Cancel);
        if (!writeToDatabaseResult.Succeeded)
        {
            _ = DeleteFileFromFileSystem(writeToStoreResult.Data.Id, Cancel);
        }

        return writeToDatabaseResult;
    }

    private async Task<Result<Guid>> VerifyFileUploadRequest(UploadFileRequest uploadFileRequest)
    {
        if (await VerifyContentType(uploadFileRequest.ContentTypeName) is { Succeeded: false } verifyContentTypeResult)
        {
            return Result<Guid>.Failure(verifyContentTypeResult);
        }

        if (await VerifyCategories(_separator, uploadFileRequest.Categories) is { Succeeded: false } verifyCategoriesResult)
        {
            return Result<Guid>.Failure(verifyCategoriesResult);
        }

        if (await VerifyAuthor(uploadFileRequest.AuthorId) is { Succeeded: false } verifyAuthorResult)
        {
            return Result<Guid>.Failure(verifyAuthorResult);
        }

        if (await _unitOfWork.GetRepository<File>().GetByNameAsync(uploadFileRequest.Name) is { })
        {
            return LoggedError<Guid>(
                Errors.Api.File.Exist,
                "File with name {fileName} already exist",
                uploadFileRequest.Name);
        }

        return Result<Guid>.Success();
    }

    private async Task<Result<ContentType>> VerifyContentType(string ContentTypeName)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByNameAsync(ContentTypeName) is not { } contentTypeModel)
        {
            return LoggedError<ContentType>(
                Errors.Api.File.ContentTypeNotFound,
                "Content type {ContentTypeName} not found.",
                ContentTypeName);
        }

        return contentTypeModel;
    }

    private async Task<Result<ICollection<Category>>> VerifyCategories(string separator, string categoriesString)
    {
        if (categoriesString is not { Length: > 0 })
        {
            return LoggedError<ICollection<Category>>(
                Errors.Api.File.CategoriesAreNotSpecified,
                "File categories are not specified");
        }

        var categoryStrings = categoriesString.Split(separator);
        var result = new List<Category>(categoryStrings.Length);
        foreach (var categoryStringId in categoryStrings)
        {
            if (!Guid.TryParse(categoryStringId, out Guid categoryId))
            {
                return LoggedError<ICollection<Category>>(
                    Errors.Api.File.CategoryNotFound,
                    "Category string {categoryStringId} is not correct Guid",
                    categoryStringId);
            }

            if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId, disableTracking: false) is not { } category)
            {
                return LoggedError<ICollection<Category>>(
                    Errors.Api.File.CategoryNotFound,
                    "File category with ID {categoryId} not found",
                    categoryId);
            }

            result.Add(category);
        }
        return result;
    }

    private async Task<Result<Author>> VerifyAuthor(Guid authorId)
    {
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(authorId) is { } author)
        {
            return author;
        }

        return LoggedError<Author>(
            Errors.Api.File.AuthorNotFound,
            "Author with ID {authorId} not found",
            authorId);
    }

    private async Task<Result<ICollection<Tag>>> GetTagsFromSeparatedStringAsync(string separator, string tagsSeparatedString)
    {
        var tagsStrings = tagsSeparatedString.Split(separator);
        var result = new List<Tag>(tagsStrings.Length);
        foreach (var tagString in tagsStrings)
        {
            if (await _unitOfWork.GetRepository<Tag>().GetByNameAsync(tagString, disableTracking: false) is not { } tag)
            {
                tag = new Tag { Name = tagString.ToLower().Trim() };
                await _unitOfWork.GetRepository<Tag>().AddAsync(tag);
            }
            result.Add(tag);
        }
        return result;
    }

    private async Task<Result<Guid>> WriteToDatabaseAsync(FileMetadata metadata, CancellationToken Cancel = default)
    {
        if (metadata is null)
        {
            throw new NullReferenceException(nameof(metadata));
        }

        if (await VerifyAuthor(metadata.AuthorId) is { Succeeded: false } verifyAuthorResult)
        {
            return Result<Guid>.Failure(verifyAuthorResult);
        }

        var verifyContentTypeResult = await VerifyContentType(metadata.ContentTypeName);
        if (!verifyContentTypeResult.Succeeded)
        {
            return Result<Guid>.Failure(verifyContentTypeResult);
        }

        var verifyCategoriesResult = await VerifyCategories(_separator, metadata.Categories);
        if (!verifyCategoriesResult.Succeeded)
        {
            return Result<Guid>.Failure(verifyCategoriesResult);
        }

        var file = _mapper.Map<File>(metadata);

        file.ContentTypeId = verifyContentTypeResult.Data.Id;

        if (verifyCategoriesResult.Data is { })
            file.Categories = verifyCategoriesResult.Data;

        if (metadata.Tags is { Length: > 0 })
        {
            var getTagsResult = await GetTagsFromSeparatedStringAsync(_separator, metadata.Tags);
            file.Tags = getTagsResult.Data;
        }

        await _unitOfWork.GetRepository<File>().AddAsync(file);
        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.File.Add,
                "File save to database  error");
        }

        _logger.LogInformation("File {fileName} created", metadata.Name);
        return file.Id;
    }

    private async Task<Result<FileMetadata>> WriteFileToStoreAsync(UploadFileRequest uploadFileRequest, Stream fileStream, CancellationToken Cancel = default)
    {
        try
        {

            Guid id = Guid.NewGuid();
            var filePath = GetFilePath(_path, id);
            var fileWriteResult = await _fileStore.WriteAsync(filePath, fileStream, Cancel).ConfigureAwait(false);

            var metadata = _mapper.Map<FileMetadata>(uploadFileRequest);
            metadata.Id = id;
            metadata.Size = fileWriteResult.Size;
            metadata.Hash = fileWriteResult.Hash;

            if (await _unitOfWork.GetRepository<File>().GetByHashAsync(metadata.Hash) is { } existingFile)
            {
                _fileStore.Delete(filePath);

                LoggedError<FileMetadata>(
                    Errors.Api.File.Exist,
                    "File with the same hash {fileHash} already exists with id: {fileId}",
                    existingFile.Hash,
                    existingFile.Id);
            }
            _logger.LogInformation("File {fileName} saved to store", metadata.Name);
            return metadata;
        }
        catch (Exception ex)
        {
            return LoggedError<FileMetadata>(
                Errors.Api.File.StoreWrite,
                ex,
                "Error when saving a file {fileName} to storage",
                uploadFileRequest.Name);
        }
    }

    private async Task<Result> WriteMetadataToStoreAsync(FileMetadata metadata, CancellationToken Cancel = default)
    {
        try
        {
            var metadataJsonString = JsonSerializer.Serialize(metadata);
            var metadataFilePath = GetMetadataPath(_path, metadata.Id);
            _ = await _fileStore.WriteAsync(metadataFilePath, metadataJsonString, Cancel).ConfigureAwait(false);
            _logger.LogInformation("File {fileName} metadata saved to store", metadata.Name);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _fileStore.Delete(GetFilePath(_path, metadata.Id));

            return LoggedError<FileMetadata>(
                Errors.Api.File.StoreWrite,
                ex,
                "Error when saving a metadata file {fileName} to storage",
                metadata.Name);
        }
    }

    private Task<Result<Guid>> DeleteFileFromFileSystem(Guid id, CancellationToken Cancel = default)
    {
        try
        {
            var filePath = GetFilePath(_path, id);
            _fileStore.Delete(filePath);

            var metadataPath = GetMetadataPath(_path, id);
            _fileStore.Delete(metadataPath);

            _logger.LogInformation("File with ID {id} deleted from file system", id);
            return Result<Guid>.Success(id).ToTask();
        }
        catch (Exception)
        {
            return LoggedError<Guid>(
                Errors.Api.File.Delete,
                "Error when deleting a file  with ID {id} from storage",
                id)
                .ToTask();
        }
    }

    private async Task<Result<Guid>> DeleteFileFromDatabase(Guid id, CancellationToken Cancel = default)
    {
        var fileRepository = _unitOfWork.GetRepository<File>();

        if (await fileRepository.GetByIdAsync(id) is { } file)
        {
            await fileRepository.DeleteAsync(file);
            await _unitOfWork.SaveContextAsync();
            _logger.LogInformation("File with ID {id} deleted from database", id);
        }
        else
            _logger.LogInformation("File with ID {id} is already deleted from database", id);

        return Result<Guid>.Success();
    }

    private static string GetFilePath(string path, Guid id)
         => Path.Combine(path, id.ToString());

    private static string GetMetadataPath(string path, Guid id)
        => Path.Combine(path, id.ToString(), ".json");
}
