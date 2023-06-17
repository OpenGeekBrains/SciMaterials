namespace SciMaterials.UrlsService.Contracts;
public interface ILinkReplaceService
{
	Task<string> RestoreLinksAsync(string text, CancellationToken Cancel = default);
	Task<string> ShortenLinksAsync(string text, CancellationToken Cancel = default);
}

