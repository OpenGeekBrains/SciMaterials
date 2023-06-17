namespace SciMaterials.UrlsService.Api;

public interface IUrlShortenerGrain : IGrainWithStringKey
{
	Task SetUrl(string fullUrl);
	Task<string> GetUrl();
}