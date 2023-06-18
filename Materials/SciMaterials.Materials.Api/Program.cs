using Microsoft.OpenApi.Models;

using SciMaterials.Materials.Api;
using SciMaterials.Materials.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddPolicy("client", b => 
	b.WithOrigins(builder.Configuration.GetValue<string>("ClientApps")?.Split(',',StringSplitOptions.TrimEntries) ?? throw new InvalidOperationException("You not setup ClientApps url's in settings file"))
	.AllowCredentials()
	.AllowAnyHeader()
	.AllowAnyMethod()));

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

app.UseCors("client");

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();
app.Run();
