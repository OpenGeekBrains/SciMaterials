using Microsoft.EntityFrameworkCore;

using SciMaterials.UrlsService.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

builder.Services.AddDbContext<UrlsStorageContext>(
			opt => opt.UseSqlServer(
				builder.Configuration.GetConnectionString("SqlServer.Debug"),
				o => o.MigrationsAssembly(typeof(UrlsStorageContext).Assembly.FullName)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger().UseSwaggerUI();
}

app.MapGet("/shorten",
	async (UrlsStorageContext storageContext, HttpRequest request, string redirect) =>
	{
		var entity = storageContext.Add(redirect);
		await storageContext.SaveChangesAsync();
		
		// Return the shortened URL for later use
		var resultBuilder = new UriBuilder($"{request.Scheme}://{request.Host.Value}")
		{
			Path = $"/go/{entity.ShortenedRouteSegment}"
		};

		return Results.Ok(resultBuilder.Uri);
	});

app.MapGet("/go/{shortenedRouteSegment}",
	async (UrlsStorageContext storageContext, string shortenedRouteSegment) =>
	{
		var entity = await storageContext.FindAsync<UrlEntity>(shortenedRouteSegment);
		if (entity is null) return Results.NotFound();

		UrlsStorageContext.UpdateLastAccess(entity);
		await storageContext.SaveChangesAsync();

		return Results.Redirect(entity.SourceAddress);
	});

app.MapGet("/original/{shortenedRouteSegment}",
	async (UrlsStorageContext storageContext, string shortenedRouteSegment) =>
	{
		var entity = await storageContext.FindAsync<UrlEntity>(shortenedRouteSegment);
		if (entity is null) return Results.NotFound();

		UrlsStorageContext.UpdateLastAccess(entity);
		await storageContext.SaveChangesAsync();

		return Results.Ok(entity.SourceAddress);
	});

app.MapGet("/database/init", async (UrlsStorageContext storageContext, ILogger<UrlsStorageContext> logger, CancellationToken cancellationToken) =>
{
	await storageContext.Initialize(logger, cancellationToken);
	return Results.Ok();
});

app.Run();
