using Microsoft.OpenApi.Models;

using SciMaterials.Identity.Api;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddCors(o => o.AddPolicy("client", b => b.WithOrigins(builder.Configuration["ClientApp"]).AllowCredentials().AllowAnyHeader().AllowAnyMethod()));

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

app.UseCors("client");

app.UseAuthentication().UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }