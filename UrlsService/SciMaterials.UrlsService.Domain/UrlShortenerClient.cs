using Flurl;
using Flurl.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SciMaterials.Contracts;
using SciMaterials.Contracts.Result;
using SciMaterials.UrlsService.Contracts;

namespace SciMaterials.UrlsService.Domain;
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

	public async Task<Result<string>> ShortenAsync(string originalUrl, CancellationToken cancellationToken)
	{
		try
		{
			var handler = _urlBase.AppendPathSegment("shorten").SetQueryParam(originalUrl);
			var response = await handler.GetStringAsync(cancellationToken);
			return Result<string>.Success(response);
		}
		catch (OperationCanceledException ex)
		{
			_logger.LogError(ex, "Url shorten was cancelled");
			return Result<string>.Failure(Errors.App.OperationCanceled);
		}
		catch (FlurlHttpException ex)
		{
			_logger.LogError(ex, "Fail on get shorten url");
			return Result<string>.Failure(Errors.App.Unhandled);
		}
	}

	public async Task<Result<string>> GetOriginalUrlAsync(string url, CancellationToken cancellationToken)
	{
		try
		{
			var handler = _urlBase.AppendPathSegment("original").SetQueryParam(url[url.LastIndexOf('/')..]);
			var response = await handler.GetStringAsync(cancellationToken);
			return Result<string>.Success(response);
		}
		catch (OperationCanceledException ex)
		{
			_logger.LogError(ex, "Get original url was cancelled");
			return Result<string>.Failure(Errors.App.OperationCanceled);
		}
		catch (FlurlHttpException ex)
		{
			_logger.LogError(ex, "Fail on get shorten url");
			return Result<string>.Failure(Errors.App.Unhandled);
		}
	}
}
