using SciMaterials.UI.MVC.API.Middlewares;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.UI.MVC;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Logging.json", true);

var config = builder.Configuration;

builder.WebHost.ConfigureKestrel(opt =>
{
    opt.Limits.MaxRequestBodySize = config.GetValue<long>("FilesApiSettings:MaxFileSize");
});

var services = builder.Services;

services.AddControllersWithViews();
services.AddRazorPages();

services.AddHttpContextAccessor();

var serverUrl = builder.WebHost.GetSetting(WebHostDefaults.ServerUrlsKey);

services
    .ConfigureFilesUploadSupport(config)
    .AddResourcesDatabaseProviders(config)
    .AddResourcesDataLayer()
    .AddResourcesApiServices(config);

services
    .AddIdentityDatabase(config)
    .AddIdentityServices(config)
    .AddIdentityClients(serverUrl);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(o =>
    {
        o.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SciMaterials",
            Version = "v1.1",
        });

        o.AddFileUploadFilter();
        o.AddOptionalRouteParameterOperationFilter();
        o.ConfigureIdentityInSwagger();
    });


var app = builder.Build();

await app.InitializeResourcesDatabaseAsync();
await app.InitializeIdentityDatabaseAsync();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapControllerRoute("default", "{controller}/{action=index}/{id?}");

app.MapPut("replace-link",
    async (string text, ILinkReplaceService linkReplaceService, LinkGenerator linkGenerator, IHttpContextAccessor context) =>
    {
        var result = await linkReplaceService.ShortenLinksAsync(text);
        return result;
    });

app.Run();

public partial class Program { }