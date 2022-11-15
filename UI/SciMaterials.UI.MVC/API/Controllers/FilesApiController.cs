using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts;
using SciMaterials.Contracts.Result;
using SciMaterials.UI.MVC.API.Filters;

namespace SciMaterials.UI.MVC.API.Controllers;

[ApiController]
[Route(WebApiRoute.Files)]
public class FilesApiController : ApiBaseController<FilesApiController>
{
    private readonly IFileService _fileService;

    public FilesApiController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetFileResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogDebug("Get all files");
        var result = await _fileService.GetAllAsync();
        return Ok(result);
    }

    /// <summary> Get paged file metadata . </summary>
    /// <param name="pageNumber"> Page number. </param>
    /// <param name="pageNumber"> Page size. </param>
    [HttpGet("page/{pageNumber}/{pageSize}")]
    [ProducesDefaultResponseType(typeof(PageResult<GetFileResponse>))]
    public async Task<IActionResult> GetPageAsync([FromRoute] int pageNumber, [FromRoute] int pageSize)
    {
        _logger.LogDebug("Get paged files");
        var result = await _fileService.GetPageAsync(pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary> Get file metadata by Id. </summary>
    /// <param name="id"> File Id. </param>
    [HttpGet("{id}/{restoreLinks?}")]
    [ProducesDefaultResponseType(typeof(Result<GetFileResponse>))]
    public async Task<IActionResult> GetById([FromRoute] Guid id, [FromRoute] bool restoreLinks = false)
    {
        _logger.LogDebug("Get by id {id}", id);
        var result = await _fileService.GetByIdAsync(id, restoreLinks);
        return Ok(result);
    }

    /// <summary> Get file metadata by Hash. </summary>
    /// <param name="id"> File hash. </param>
    [HttpGet("GetByHash/{hash}/{restoreLinks?}")]
    [ProducesDefaultResponseType(typeof(Result<GetFileResponse>))]
    public async Task<IActionResult> GetByHash([FromRoute] string hash, [FromRoute] bool restoreLinks = false)
    {
        _logger.LogDebug("Get by hash {hash}", hash);
        var result = await _fileService.GetByHashAsync(hash, restoreLinks);
        return Ok(result);
    }


    [HttpPut("Edit")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> EditAsync([FromBody] EditFileRequest request)
    {
        var result = await _fileService.EditAsync(request);
        return Ok(result);
    }

    [HttpGet("DownloadByHash/{hash}")]
    [ProducesDefaultResponseType(typeof(FileStreamResult))]
    public async Task<IActionResult> DownloadByHash([FromRoute] string hash)
    {
        _logger.LogDebug("Get file by hash");

        var fileStreamInfoResult = await _fileService.DownloadByHash(hash);
        if (fileStreamInfoResult is { Succeeded: true })
            return File(fileStreamInfoResult.Data.FileStream, fileStreamInfoResult.Data.ContentTypeName, fileStreamInfoResult.Data.FileName);

        return Ok(fileStreamInfoResult);
    }

    [HttpGet("DownloadById/{id}")]
    [ProducesDefaultResponseType(typeof(FileStreamResult))]
    public async Task<IActionResult> DownloadByIdAsync([FromRoute] Guid id)
    {
        _logger.LogDebug("Download by Id");

        var fileStreamInfoResult = await _fileService.DownloadById(id);
        if (fileStreamInfoResult is { Succeeded: true })
            return File(fileStreamInfoResult.Data.FileStream, fileStreamInfoResult.Data.ContentTypeName, fileStreamInfoResult.Data.FileName);

        return Ok(fileStreamInfoResult);
    }

    [DisableFormValueModelBinding]
    [HttpPost("Upload")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> UploadAsync()
    {
        _logger.LogDebug("Upload file");
        var request = HttpContext.Request;

        if (!request.HasFormContentType ||
           !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
           string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
        {
            return new UnsupportedMediaTypeResult();
        }

        var boundary = mediaTypeHeader.Boundary is { Length: > 2 } b && b[0] == '"' && b[^1] == '"'
            ? b.Subsegment(1, b.Length - 2)
            : mediaTypeHeader.Boundary;

        var boundaryValue = boundary.Value;
        _logger.LogInformation("Область загрузки {0}", boundaryValue);

        var reader = new MultipartReader(boundaryValue, request.Body);
        var section = await reader.ReadNextSectionAsync();

        while (section != null)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                out var contentDisposition);

            if (hasContentDispositionHeader && contentDisposition is not null && contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value))
            {
                _logger.LogInformation("Section contains file {file}", contentDisposition.FileName.Value);
                if (section.Headers is null
                    || !section.Headers.ContainsKey("Metadata")
                    || System.Text.Json.JsonSerializer.Deserialize<UploadFileRequest>(section.Headers["Metadata"]) is not { } uploadFileRequest)
                    return Ok(Result.Failure(Errors.Api.File.MissingMetadata));

                var result = await _fileService.UploadAsync(section.Body, uploadFileRequest).ConfigureAwait(false);
                return Ok(result);
            }

            section = await reader.ReadNextSectionAsync();
        }

        return Ok(Result.Failure(Errors.Api.File.MissingSection));
    }

    /// <summary> Delete a file. </summary>
    /// <param name="id"> File Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _fileService.DeleteAsync(id);
        return Ok(result);
    }
}
