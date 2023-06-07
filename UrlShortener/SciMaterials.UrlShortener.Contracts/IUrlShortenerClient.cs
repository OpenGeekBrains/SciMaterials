using SciMaterials.Contracts.Result;

namespace SciMaterials.UrlShortener.Contracts;
public interface IUrlShortenerClient
{
	string ShortenedUrlBase { get; }

	Task<Result<string>> GetOriginalUrlAsync(string url);
	Task<Result<string>> ShortenAsync(string originalUrl);
}