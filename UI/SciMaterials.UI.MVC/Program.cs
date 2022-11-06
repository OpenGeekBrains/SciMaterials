using SciMaterials.Services.API.Configuration;
using SciMaterials.Services.API.Extensions;
using SciMaterials.UI.MVC.API.Middlewares;
using SciMaterials.UI.MVC.API.Extensions;
using SciMaterials.Services.Database.Extensions;
using SciMaterials.UI.MVC.Identity.Extensions;
using SciMaterials.WebApi.Clients.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.WebHost.ConfigureKestrel(opt =>
{
    var api_settings = config.GetSection(ApiSettings.SectionName);
    //var fileSize = apiSettings.GetValue<long>("MaxFileSize");
    opt.Limits.MaxRequestBodySize = api_settings.GetValue<long>("MaxFileSize");
});

var services = builder.Services;

services.AddControllersWithViews();
services.AddRazorPages();

services.ConfigureApplication(config);
services.AddApiServices(config);

services.AddAuthApiServices(config);
services.AddAuthDbInitializer();
services.AddAuthUtils();
services.AddIdentityClients(new Uri(config["WebAPI"]));
services.AddAuthJwtAndSwaggerApiServices(builder.Configuration);

builder.Services.AddCors(o => o.AddPolicy("test", p => p.WithOrigins("http://localhost:5159").AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

await app.InitializeDbAsync(config);

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/MVC/Error");
}

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

app.UseCors("test");

app.UseRouting();

app.UseAuthentication();
        
app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapRazorPages();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.MapControllerRoute("default", "{controller}/{action=index}/{id?}");

app.Run();

public partial class Program { }