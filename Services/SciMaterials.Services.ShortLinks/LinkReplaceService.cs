using System.Text;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.ShortLinks;

using SciMaterials.DAL.Models;

namespace SciMaterials.Services.ShortLinks;

public class LinkReplaceService : ILinkReplaceService
{
    private const string sourceLinkPattern = @"((http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)";
    private const string hashPattern = @"(\w{5,})";
    private readonly ILinkShortCutService _linkShortCut;
    private readonly ILogger<LinkReplaceService> _logger;

    public LinkReplaceService(ILinkShortCutService linkShortCut, ILogger<LinkReplaceService> logger)
    {
        _linkShortCut = linkShortCut;
        _logger = logger;
    }

    public async Task<string> ShortenLinksAsync(string text, CancellationToken Cancel = default)
    {
        var result = await ReplaceLinksAsync(
            text,
            sourceLinkPattern,
            async (matchText, Cancel) => await _linkShortCut.AddAsync(matchText, Cancel),
            Cancel);
        return result;
    }
    public async Task<string> RestoreLinksAsync(string text, CancellationToken Cancel = default)
    {
        var shortLinkPattern = _linkShortCut.GetLinkBasePath() + hashPattern;
        var result = await ReplaceLinksAsync(
            text,
            shortLinkPattern,
            async (matchText, Cancel) =>
                await _linkShortCut.GetAsync(matchText, false, Cancel),
            Cancel);
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

