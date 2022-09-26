using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SciMaterials.API.Data.Interfaces;
using SciMaterials.API.Mappings;
using SciMaterials.API.Services.Interfaces;

namespace SciMaterials.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly ILogger<FilesController> _logger;
    private readonly IFileService _fileService;
    private readonly IFileRepository _fileRepository;

    private void LogError(Exception ex, [CallerMemberName] string methodName = null!)
        => _logger.LogError(ex, "ошибка выполнения {error}", methodName);

    public FilesController(ILogger<FilesController> logger, IFileService fileService, IFileRepository fileRepository)
    {
        _logger = logger;
        _fileService = fileService;
        _fileRepository = fileRepository;
    }

    [HttpGet("{hash}")]
    public IActionResult GetByHash([FromRoute] string hash)
    {
        try
        {
            var model = _fileRepository.GetByHash(hash);
            if (model is null)
                return BadRequest($"File with hash({hash}) not found");

            var fileStream = _fileService.GetFileAsStream(hash);
            return File(fileStream, model.ContentType, model.FileName);
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> UploadAsync()
    {
        try
        {
            var request = HttpContext.Request;

            if (!request.HasFormContentType ||
               !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
               string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    var result = await _fileService.UploadAsync(section.Body, contentDisposition.FileName.Value, section.ContentType ?? "application/octet-stream").ConfigureAwait(false);
                    return Ok(result.ToViewModel());
                }

                section = await reader.ReadNextSectionAsync();
            }

            return BadRequest("No files data in the request.");
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

}
