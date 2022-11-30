using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Contracts.API.Models;
using System.Text.Json;
using SciMaterials.Contracts;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Extensions;
using SciMaterials.DAL.Resources.UnitOfWork;
using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.Services.API.Services.Files;

public class FileService : ApiServiceBase, IFileService
{
    private readonly IFileStore _FileStore;
    private readonly ILinkReplaceService _LinkReplaceService;
    private readonly string _Path;
    private readonly string _Separator;

    public FileService(
        ApiSettings ApiSettings,
        IFileStore FileStore,
        ILinkReplaceService LinkReplaceService,
        IUnitOfWork<SciMaterialsContext> Database,
        IMapper Mapper,
        ILogger<FileService> Logger)
        : base(Database, Mapper, Logger)
    {
        _FileStore          = FileStore;
        _LinkReplaceService = LinkReplaceService;
        _Path               = ApiSettings.BasePath;
        _Separator          = ApiSettings.Separator;

        if (_Path is not { Length: > 0 })
            throw new ArgumentNullException(nameof(ApiSettings.BasePath));
    }

    public async Task<Result<IEnumerable<GetFileResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var repository = Database.GetRepository<File>().Include();
        var files      = await repository.GetAllAsync();
        var result     = _Mapper.Map<List<GetFileResponse>>(files);
        return result;
    }

    public async Task<PageResult<GetFileResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var repository  = Database.GetRepository<File>().Include();
        var files       = await repository.GetPageAsync(PageNumber, PageSize);
        var total_count = await repository.GetCountAsync();
        var result      = _Mapper.Map<List<GetFileResponse>>(files);
        return (result, total_count);
    }

    public async Task<Result<GetFileResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        var repository = Database.GetRepository<File>().Include();

        if (await repository.GetByIdAsync(id) is not { } file)
            return LoggedError<GetFileResponse>(Errors.Api.File.NotFound, "File with ID {id} not found", id);

        var result = _Mapper.Map<GetFileResponse>(file);
        return result;
    }

    public async Task<Result<GetFileResponse>> GetByIdAsync(Guid id, bool RestoreLinks = false, CancellationToken Cancel = default)
    {
        var result = await GetByIdAsync(id, Cancel);

        if (RestoreLinks && result.Succeeded && result.Data?.Description is { Length: > 0 })
            result.Data.Description = await _LinkReplaceService.RestoreLinksAsync(result.Data.Description, Cancel).ConfigureAwait(false);

        return result;
    }

    public async Task<Result<GetFileResponse>> GetByHashAsync(string Hash, bool RestoreLinks = false, CancellationToken Cancel = default)
    {
        var repository = Database.GetRepository<File>().Include();
        if (await repository.GetByHashAsync(Hash) is not { } file)
            return LoggedError<GetFileResponse>(Errors.Api.File.NotFound, "File with hash {hash} not found", Hash);

        var result = _Mapper.Map<GetFileResponse>(file);

        if (RestoreLinks && result.Description is { Length: > 0 })
            result.Description = await _LinkReplaceService.RestoreLinksAsync(result.Description, Cancel).ConfigureAwait(false);

        return result;
    }

    public async Task<Result<Guid>> EditAsync(EditFileRequest EditFileRequest, CancellationToken Cancel = default)
    {
        var repository = Database.GetRepository<File>().Tracking();
        if (await repository.GetByIdAsync(EditFileRequest.Id) is not { } existing_file)
            return LoggedError<Guid>(Errors.Api.File.NotFound, "File with ID {fileId} not found", EditFileRequest.Id);

        var verify_categories_result = await VerifyCategories(_Separator, EditFileRequest.Categories);
        if (!verify_categories_result.Succeeded)
            return Result<Guid>.Failure(verify_categories_result);

        var file = _Mapper.Map(EditFileRequest, existing_file);

        if (verify_categories_result.Data is { })
        {
            file.Categories.Clear();
            file.Categories = verify_categories_result.Data;
        }

        if (EditFileRequest.Tags is { Length: > 0 })
        {
            var get_tags_result = await GetTagsFromSeparatedStringAsync(_Separator, EditFileRequest.Tags);
            file.Tags?.Clear();
            file.Tags = get_tags_result.Data;
        }

        if (file.Description is { Length: > 0 })
            file.Description = await _LinkReplaceService.ShortenLinksAsync(file.Description, Cancel).ConfigureAwait(false);

        await repository.UpdateAsync(file);
        if (await Database.SaveContextAsync() == 0)
            return LoggedError<Guid>(
                Errors.Api.File.Update,
                "File {fileName} update error",
                EditFileRequest.Name);

        _Logger.LogInformation("File {fileName} updated.", EditFileRequest.Name);
        return file.Id;
    }

    public async Task<Result<FileStreamInfo>> DownloadByHash(string hash, CancellationToken Cancel = default)
    {
        if (await GetByHashAsync(hash, RestoreLinks: false, Cancel) is not { } get_file_response)
            return LoggedError<FileStreamInfo>(
                Errors.Api.File.NotFound,
                "File with hash {fileHash} not found",
                hash);

        var read_from_path = Path.Combine(_Path, get_file_response.Data!.Id.ToString());
        try
        {
            var file_stream = _FileStore.OpenRead(read_from_path);
            return new FileStreamInfo(get_file_response.Data.Name, get_file_response.Data.ContentTypeName, file_stream);
        }
        catch (Exception ex)
        {
            return LoggedError<FileStreamInfo>(Errors.Api.File.Download, ex, "Download by hash {fileHash} error", hash);
        }
    }

    public async Task<Result<FileStreamInfo>> DownloadById(Guid id, CancellationToken Cancel = default)
    {
        if (await GetByIdAsync(id, Cancel) is not { } get_file_response)
            return LoggedError<FileStreamInfo>(
                Errors.Api.File.NotFound,
                "File with ID {fileId} not found",
                id);

        var read_from_path = Path.Combine(_Path, id.ToString());
        try
        {
            var file_stream = _FileStore.OpenRead(read_from_path);
            return new FileStreamInfo(get_file_response.Data.Name, get_file_response.Data.ContentTypeName, file_stream);
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
        if (await DeleteFileFromFileSystem(id, Cancel) is { Succeeded: false } delete_from_file_system_result)
            return Result<Guid>.Failure(delete_from_file_system_result);

        return await DeleteFileFromDatabase(id, Cancel);
    }

    public async Task<Result<Guid>> UploadAsync(Stream FileStream, UploadFileRequest UploadFileRequest, CancellationToken Cancel = default)
    {
        if (await VerifyFileUploadRequest(UploadFileRequest) is { Succeeded: false } verify_file_upload_request_result)
            return Result<Guid>.Failure(verify_file_upload_request_result);

        if (UploadFileRequest.Description is { Length: > 0 })
        {
            var description_with_short_links = await _LinkReplaceService.ShortenLinksAsync(UploadFileRequest.Description, Cancel);
            UploadFileRequest.Description = description_with_short_links;
        }

        var write_to_store_result = await WriteFileToStoreAsync(UploadFileRequest, FileStream, Cancel);
        if (!write_to_store_result.Succeeded)
            return Result<Guid>.Failure(write_to_store_result);

        var write_metadata_to_store_result = await WriteMetadataToStoreAsync(write_to_store_result.Data, Cancel);
        if (!write_metadata_to_store_result.Succeeded)
            return Result<Guid>.Failure(write_metadata_to_store_result);

        var write_to_database_result = await WriteToDatabaseAsync(write_to_store_result.Data, Cancel);
        if (!write_to_database_result.Succeeded)
            _ = DeleteFileFromFileSystem(write_to_store_result.Data.Id, Cancel);

        return write_to_database_result;
    }

    private async Task<Result<Guid>> VerifyFileUploadRequest(UploadFileRequest UploadFileRequest)
    {
        if (await VerifyContentType(UploadFileRequest.ContentTypeName) is { Succeeded: false } verify_content_type_result)
            return Result<Guid>.Failure(verify_content_type_result);

        if (await VerifyCategories(_Separator, UploadFileRequest.Categories) is { Succeeded: false } verify_categories_result)
            return Result<Guid>.Failure(verify_categories_result);

        if (await VerifyAuthor(UploadFileRequest.AuthorId) is { Succeeded: false } verify_author_result)
            return Result<Guid>.Failure(verify_author_result);

        if (await Database.GetRepository<File>().GetByNameAsync(UploadFileRequest.Name) is { })
            return LoggedError<Guid>(Errors.Api.File.Exist, "File with name {fileName} already exist", UploadFileRequest.Name);

        return Result<Guid>.Success();
    }

    private async Task<Result<ContentType>> VerifyContentType(string ContentTypeName)
    {
        if (await Database.GetRepository<ContentType>().GetByNameAsync(ContentTypeName) is not { } content_type_model)
            return LoggedError<ContentType>(Errors.Api.File.ContentTypeNotFound, "Content type {ContentTypeName} not found.", ContentTypeName);

        return content_type_model;
    }

    private async Task<Result<ICollection<Category>>> VerifyCategories(string Separator, string CategoriesString)
    {
        if (CategoriesString is not { Length: > 0 })
            return LoggedError<ICollection<Category>>(Errors.Api.File.CategoriesAreNotSpecified, "File categories are not specified");

        var category_strings = CategoriesString.Split(Separator);
        var result           = new List<Category>(category_strings.Length);
        foreach (var category_string_id in category_strings)
        {
            if (!Guid.TryParse(category_string_id, out var category_id))
                return LoggedError<ICollection<Category>>(
                    Errors.Api.File.CategoryNotFound,
                    "Category string {categoryStringId} is not correct Guid",
                    category_string_id);

            var repository = Database.GetRepository<Category>().Tracking();
            if (await repository.GetByIdAsync(category_id) is not { } category)
                return LoggedError<ICollection<Category>>(Errors.Api.File.CategoryNotFound, "File category with ID {categoryId} not found", category_id);

            result.Add(category);
        }

        return result;
    }

    private async Task<Result<Author>> VerifyAuthor(Guid AuthorId)
    {
        if (await Database.GetRepository<Author>().GetByIdAsync(AuthorId) is { } author)
            return author;

        return LoggedError<Author>(
            Errors.Api.File.AuthorNotFound,
            "Author with ID {authorId} not found",
            AuthorId);
    }

    private async Task<Result<ICollection<Tag>>> GetTagsFromSeparatedStringAsync(string separator, string TagsSeparatedString)
    {
        var tags_strings = TagsSeparatedString.Split(separator);
        var result       = new List<Tag>(tags_strings.Length);
        foreach (var tag_string in tags_strings)
        {
            var repository = Database.GetRepository<Tag>().Tracking();
            if (await repository.GetByNameAsync(tag_string) is not { } tag)
            {
                tag = new Tag { Name = tag_string.ToLower().Trim() };
                await repository.AddAsync(tag);
            }

            result.Add(tag);
        }

        return result;
    }

    private async Task<Result<Guid>> WriteToDatabaseAsync(FileMetadata metadata, CancellationToken Cancel = default)
    {
        if (metadata is null)
            throw new NullReferenceException(nameof(metadata));

        if (await VerifyAuthor(metadata.AuthorId) is { Succeeded: false } verify_author_result)
            return Result<Guid>.Failure(verify_author_result);

        var verify_content_type_result = await VerifyContentType(metadata.ContentTypeName);
        if (!verify_content_type_result.Succeeded)
            return Result<Guid>.Failure(verify_content_type_result);

        var verify_categories_result = await VerifyCategories(_Separator, metadata.Categories);
        if (!verify_categories_result.Succeeded)
            return Result<Guid>.Failure(verify_categories_result);

        var file = _Mapper.Map<File>(metadata);

        file.ContentTypeId = verify_content_type_result.Data.Id;

        if (verify_categories_result.Data is { })
            file.Categories = verify_categories_result.Data;

        if (metadata.Tags is { Length: > 0 })
        {
            var get_tags_result = await GetTagsFromSeparatedStringAsync(_Separator, metadata.Tags);
            file.Tags = get_tags_result.Data;
        }

        await Database.GetRepository<File>().AddAsync(file);
        if (await Database.SaveContextAsync() == 0)
            return LoggedError<Guid>(Errors.Api.File.Add, "File save to database  error");

        _Logger.LogInformation("File {fileName} created", metadata.Name);
        return file.Id;
    }

    private async Task<Result<FileMetadata>> WriteFileToStoreAsync(UploadFileRequest UploadFileRequest, Stream FileStream, CancellationToken Cancel = default)
    {
        try
        {
            var id                = Guid.NewGuid();
            var file_path         = GetFilePath(_Path, id);
            var file_write_result = await _FileStore.WriteAsync(file_path, FileStream, Cancel).ConfigureAwait(false);

            var metadata = _Mapper.Map<FileMetadata>(UploadFileRequest);
            metadata.Id   = id;
            metadata.Size = file_write_result.Size;
            metadata.Hash = file_write_result.Hash;

            if (await Database.GetRepository<File>().GetByHashAsync(metadata.Hash) is { } existing_file)
            {
                _FileStore.Delete(file_path);

                LoggedError<FileMetadata>(
                    Errors.Api.File.Exist,
                    "File with the same hash {fileHash} already exists with id: {fileId}",
                    existing_file.Hash,
                    existing_file.Id);
            }

            _Logger.LogInformation("File {fileName} saved to store", metadata.Name);
            return metadata;
        }
        catch (Exception ex)
        {
            return LoggedError<FileMetadata>(
                Errors.Api.File.StoreWrite,
                ex,
                "Error when saving a file {fileName} to storage",
                UploadFileRequest.Name);
        }
    }

    private async Task<Result> WriteMetadataToStoreAsync(FileMetadata metadata, CancellationToken Cancel = default)
    {
        try
        {
            var metadata_json_string = JsonSerializer.Serialize(metadata);
            var metadata_file_path   = GetMetadataPath(_Path, metadata.Id);
            _ = await _FileStore.WriteAsync(metadata_file_path, metadata_json_string, Cancel).ConfigureAwait(false);
            _Logger.LogInformation("File {fileName} metadata saved to store", metadata.Name);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _FileStore.Delete(GetFilePath(_Path, metadata.Id));

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
            var file_path = GetFilePath(_Path, id);
            _FileStore.Delete(file_path);

            var metadata_path = GetMetadataPath(_Path, id);
            _FileStore.Delete(metadata_path);

            _Logger.LogInformation("File with ID {id} deleted from file system", id);
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
        var file_repository = Database.GetRepository<File>();

        if (await file_repository.GetByIdAsync(id) is { } file)
        {
            await file_repository.DeleteAsync(file);
            await Database.SaveContextAsync();
            _Logger.LogInformation("File with ID {id} deleted from database", id);
        }
        else
            _Logger.LogInformation("File with ID {id} is already deleted from database", id);

        return Result<Guid>.Success();
    }

    private static string GetFilePath(string path, Guid id) => Path.Combine(path, id.ToString());

    private static string GetMetadataPath(string path, Guid id) => Path.Combine(path, id.ToString(), ".json");
}
