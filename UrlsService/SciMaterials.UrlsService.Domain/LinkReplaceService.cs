using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.Result;
using SciMaterials.UrlsService.Contracts;

namespace SciMaterials.UrlsService.Domain;

public class LinkReplaceService : ILinkReplaceService
{
	private const string _sourceLinkPattern = @"((http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)";
	private const string _hashPattern = @"(\w{5,})";

	private readonly IUrlShortenerClient _urlShortenerClient;
	private readonly ILogger<LinkReplaceService> _logger;

	public LinkReplaceService(IUrlShortenerClient urlShortener, ILogger<LinkReplaceService> logger)
	{
		_urlShortenerClient = urlShortener;
		_logger = logger;
	}

	public async Task<string> ShortenLinksAsync(string text, CancellationToken Cancel = default)
	{
		var result = await ReplaceLinksAsync(text, _sourceLinkPattern, _urlShortenerClient.ShortenAsync, Cancel);
		return result;
	}
	public async Task<string> RestoreLinksAsync(string text, CancellationToken Cancel = default)
	{
		var shortLinkPattern = _urlShortenerClient.ShortenedUrlBase + _hashPattern;
		var result = await ReplaceLinksAsync(text, shortLinkPattern, _urlShortenerClient.GetOriginalUrlAsync, Cancel);
		return result;
	}

	private async Task<string> ReplaceLinksAsync(
		string text,
		string pattern,
		Func<string, CancellationToken, Task<Result<string>>> linkResolver,
		CancellationToken Cancel)
	{
		var regex = new Regex(pattern);
		var sb = new StringBuilder();
		var lastIndex = 0;
		foreach (var match in regex.Matches(text).Cast<Match>())
		{
			_logger.LogDebug("Short link found: {link}", match.Value);
			sb.Append(text, lastIndex, match.Index - lastIndex);
			var linkResult = await linkResolver(match.Value, Cancel);
			if (linkResult.Succeeded)
			{
				sb.Append(linkResult.Data);
				lastIndex = match.Index + match.Length;
			}
		}
		sb.Append(text, lastIndex, text.Length - lastIndex);

		return sb.ToString();
	}
}

