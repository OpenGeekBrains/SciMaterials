using Orleans.Runtime;

namespace SciMaterials.UrlsService.Api;

public class UrlShortenerGrain : Grain, IUrlShortenerGrain
{
	private readonly IPersistentState<UrlDetails> _state;

	public UrlShortenerGrain([PersistentState(stateName: "url", storageName: "urls")] IPersistentState<UrlDetails> state)
	{
		_state = state;
	}

	public async Task SetUrl(string fullUrl)
	{
		_state.State = new UrlDetails()
		{
			ShortenedRouteSegment = this.GetPrimaryKeyString(),
			FullUrl = fullUrl
		};
		await _state.WriteStateAsync();
	}

	public async Task<string> GetUrl()
	{
		_state.State.AccessCount++;
		_state.State.LastAccess = DateTime.UtcNow;
		await _state.WriteStateAsync();

		return _state.State.FullUrl;
	}
}