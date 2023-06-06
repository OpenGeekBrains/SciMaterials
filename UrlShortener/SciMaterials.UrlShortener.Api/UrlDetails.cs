namespace SciMaterials.UrlShortener.Api;

[GenerateSerializer]
public class UrlDetails
{
	[Id(0)]
	public string FullUrl { get; set; } = string.Empty;
	[Id(1)]
	public string ShortenedRouteSegment { get; set; } = string.Empty;

	[Id(2)]
	public int AccessCount { get; set; }
	[Id(3)]
	public DateTime? LastAccess { get; set; }
}