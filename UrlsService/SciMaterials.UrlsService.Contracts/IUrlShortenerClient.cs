using SciMaterials.Contracts.Result;

namespace SciMaterials.UrlsService.Contracts;
public interface IUrlShortenerClient
{
	string ShortenedUrlBase { get; }

	Task<Result<string>> GetOriginalUrlAsync(string url, CancellationToken cancellationToken);
	Task<Result<string>> ShortenAsync(string originalUrl, CancellationToken cancellationToken);
}