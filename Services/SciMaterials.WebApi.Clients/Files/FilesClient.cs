using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using System.Globalization;
using System.Text.Json;
using System.Net.Http.Json;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.WebApi.Clients.Files;

namespace SciMaterials.WebApi.Clients.Files;

public class FilesClient :
    ApiModifiedClientBase<Guid, GetFileResponse, EditFileRequest>,
    IFilesClient
{
    public FilesClient(HttpClient httpClient, ILogger<FilesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Files;

    public async Task<Result<GetFileResponse>> GetByHashIdAsync(string hash, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Get file by hash {hash}", hash);

        var result = await _httpClient.GetFromJsonAsync<Result<GetFileResponse>>($"{WebApiRoute.Files}/{hash}", Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }

    public async Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Upload file {file}", uploadFileRequest.Name);

        var matadata = JsonSerializer.Serialize(uploadFileRequest);

        using var multipartFormDataContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.Add("Metadata", matadata);
        multipartFormDataContent.Add(streamContent, "file", uploadFileRequest.Name);

        var response = await _httpClient.PostAsync($"{WebApiRoute.Files}/upload", multipartFormDataContent, Cancel);
        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<Result<Guid>>(cancellationToken: Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");

        return result;
    }

    public async Task<Result<FileStreamInfo>> DownloadByHashAsync(string hash, CancellationToken Cancel = default)
    {
        _logger.LogDebug("Download by hash {hash}", hash);

        return await Download($"{WebApiRoute.Files}/DownloadByHash/{hash}", Cancel);
    }

    public async Task<Result<FileStreamInfo>> DownloadByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        _logger.LogDebug("Download by Id {id}", id);

        return await Download($"{WebApiRoute.Files}/DownloadById/{id}", Cancel);
    }

    private async Task<Result<FileStreamInfo>> Download(string path, CancellationToken Cancel = default)
    {
        var response = await _httpClient.GetAsync(path, Cancel);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentType is not null
            && response.Content.Headers.ContentDisposition is not null
            && response.Content.Headers.ContentDisposition.DispositionType.Equals("attachment"))
        {
            var stream = await response.Content.ReadAsStreamAsync()
                ?? throw new InvalidOperationException("No response received from the server.");

            return new FileStreamInfo(
                response.Content.Headers.ContentDisposition.FileName,
                response.Content.Headers.ContentType.MediaType,
                stream);
        }

        var result = await response.Content.ReadFromJsonAsync<Result<FileStreamInfo>>(cancellationToken: Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }
}
