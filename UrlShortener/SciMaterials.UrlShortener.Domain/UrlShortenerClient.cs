using Flurl;
using Flurl.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SciMaterials.Contracts;
using SciMaterials.Contracts.Result;
using SciMaterials.UrlShortener.Contracts;

namespace SciMaterials.UrlShortener.Domain;
public class UrlShortenerClient : IUrlShortenerClient
{
	private readonly string _urlBase;
	private readonly string _shortenedUrlBase;
	private readonly ILogger<UrlShortenerClient> _logger;

	public UrlShortenerClient(IConfiguration configuration, ILogger<UrlShortenerClient> logger)
	{
		_urlBase = configuration["UrlShortenerApi"];
		_shortenedUrlBase = _urlBase.AppendPathSegment("go");
		_logger = logger;
	}

	public string ShortenedUrlBase => _shortenedUrlBase;

	public async Task<Result<string>> ShortenAsync(string originalUrl)
	{
		try
		{
			var handler = _urlBase.AppendPathSegment("shorten").SetQueryParam(originalUrl);
			var response = await handler.GetStringAsync();
			return Result<string>.Success(response);
		}
		catch (FlurlHttpException ex)
		{
			_logger.LogError(ex, "Fail on get shorten url");
			return Result<string>.Failure(Errors.App.Unhandled);
		}
	}

	public async Task<Result<string>> GetOriginalUrlAsync(string url)
	{
		try
		{
			var handler = _urlBase.AppendPathSegment("original").SetQueryParam(url[url.LastIndexOf('/')..]);
			var response = await handler.GetStringAsync();
			return Result<string>.Success(response);
		}
		catch (FlurlHttpException ex)
		{
			_logger.LogError(ex, "Fail on get shorten url");
			return Result<string>.Failure(Errors.App.Unhandled);
		}
	}
}
