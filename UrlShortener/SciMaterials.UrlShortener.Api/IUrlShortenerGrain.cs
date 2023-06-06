namespace SciMaterials.UrlShortener.Api;

public interface IUrlShortenerGrain : IGrainWithStringKey
{
	Task SetUrl(string fullUrl);
	Task<string> GetUrl();
}