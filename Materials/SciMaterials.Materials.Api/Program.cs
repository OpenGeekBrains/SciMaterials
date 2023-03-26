using Microsoft.OpenApi.Models;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Materials.Api;
using SciMaterials.Materials.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen(o =>
{
	o.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "SciMaterials Materials",
		Version = "v1.1",
	});

	o.AddFileUploadFilter();
	o.AddOptionalRouteParameterOperationFilter();
});

var config = builder.Configuration;
var services = builder.Services;

builder.WebHost.ConfigureKestrel(opt =>
{
	opt.Limits.MaxRequestBodySize = config.GetValue<long>("FilesApiSettings:MaxFileSize");
});

services.AddHttpContextAccessor();

services
	.ConfigureFilesUploadSupport(config)
	.AddResourcesDatabaseProviders(config)
	.AddResourcesDataLayer()
	.AddResourcesApiServices(config);

var app = builder.Build();

await app.InitializeResourcesDatabaseAsync();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger().UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapPut("replace-link",
	async (string text, ILinkReplaceService linkReplaceService, LinkGenerator linkGenerator, IHttpContextAccessor context) =>
	{
		var result = await linkReplaceService.ShortenLinksAsync(text);
		return result;
	});
app.MapControllers();
app.Run();
