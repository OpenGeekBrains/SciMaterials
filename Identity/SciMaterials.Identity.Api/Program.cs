using Microsoft.OpenApi.Models;

using SciMaterials.Identity.Api;
using SciMaterials.Services.Identity.API;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen(o =>
{
	o.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "SciMaterials Identity",
		Version = "v1.1",
	});

	o.ConfigureIdentityInSwagger();
});
var serverUrl = builder.WebHost.GetSetting(WebHostDefaults.ServerUrlsKey);

builder.Services
	.AddIdentityDatabase(config)
	.AddIdentityServices(config)
	.AddIdentityClients(serverUrl);

var app = builder.Build();

await app.InitializeIdentityDatabaseAsync();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger().UseSwaggerUI();
}

app.UseAuthentication().UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }