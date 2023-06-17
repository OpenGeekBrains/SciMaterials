using SciMaterials.UrlsService.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

builder.Host.UseOrleans(sb =>
{
	sb.AddAdoNetGrainStorage("urls", options =>
	{
		options.Invariant = "System.Data.SqlClient";
		options.ConnectionString = builder.Configuration.GetConnectionString("SqlServer.Debug");
	});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger().UseSwaggerUI();
}

app.MapGet("/shorten",
	async (IGrainFactory grains, HttpRequest request, string redirect) =>
	{
		// Create a unique, short ID
		var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");

		// Create and persist a grain with the shortened ID and full URL
		var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
		await shortenerGrain.SetUrl(redirect);

		// Return the shortened URL for later use
		var resultBuilder = new UriBuilder($"{request.Scheme}://{request.Host.Value}")
		{
			Path = $"/go/{shortenedRouteSegment}"
		};

		return Results.Ok(resultBuilder.Uri);
	});

app.MapGet("/go/{shortenedRouteSegment}",
	async (IGrainFactory grains, string shortenedRouteSegment) =>
	{
		// Retrieve the grain using the shortened ID and redirect to the original URL        
		var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
		var url = await shortenerGrain.GetUrl();

		return Results.Redirect(url);
	});

app.MapGet("/original/{shortenedRouteSegment}",
	async (IGrainFactory grains, string shortenedRouteSegment) =>
	{
		// Retrieve the grain using the shortened ID and redirect to the original URL        
		var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
		var url = await shortenerGrain.GetUrl();

		return Results.Ok(url);
	});

app.Run();
