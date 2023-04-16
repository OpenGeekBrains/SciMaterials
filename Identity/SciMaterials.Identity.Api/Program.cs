using Microsoft.OpenApi.Models;

using SciMaterials.Identity.Api;

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
builder.Services
	.AddIdentityDatabase(config)
	.AddIdentityServices(config);

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