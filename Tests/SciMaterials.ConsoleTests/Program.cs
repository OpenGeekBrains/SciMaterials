#region usings

using System.Threading.Channels;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.ConsoleTests;
using SciMaterials.ConsoleTests.Extensions;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Contracts.ShortLinks.Settings;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Services.API.Extensions;
using SciMaterials.Services.Database.Services.DbInitialization;
using SciMaterials.Services.ShortLinks;
using SciMaterials.WebApi.Clients.Extensions;
using SciMaterials.WebApi.Clients.Files;
using File = SciMaterials.DAL.Models.File;
#endregion

const string path = @"d:\tmp\test.txt";
const string baseAddress = "http://localhost:5185/";

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices)
    .ConfigureAppConfiguration(app => app.AddJsonFile("appsettings.json")
        .AddUserSecrets<Program>());

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.Configure<LinkShortCutOptions>(context.Configuration.GetSection("LinkShortCutOptions"));
    services.AddContextMultipleProviders(context);
    services.AddTransient<IDbInitializer, DbInitializer>();
    services.AddScoped<AddFileWithCategories>();
    services.AddApiServices(context.Configuration);
    services.AddScoped<GetAllFilesTest>();
    services.AddScoped<GetFileByIdTest>();
    services.AddScoped<EditFilesTest>();
    services.AddScoped<SendFileTest>();
    services.AddScoped<DownloadFileByIdTest>();
    services.AddScoped<ILinkReplaceService, LinkReplaceService>();
    services.AddScoped<ILinkShortCutService, LinkShortCutService>();
    services.AddApiClients(new Uri(baseAddress));
    services.AddHttpContextAccessor();

}
using IHost host = CreateHostBuilder(args).Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var linkReplaceService = scope.ServiceProvider.GetService<ILinkReplaceService>();
    var text = "The target endpoint might be prepared to accept the <code>application/json</code> content type for additional data. It needs <a href=\"https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding\" target=\"_blank\" rel=\"noreferrer\">custom model binders</a> that deserializes the JSON content to the target type. In this case, the <code>Data</code> property is decorated with the <code>ModelBinder</code> attribute that takes the type of a custom binder.";
    Console.WriteLine(text);

    var updatedText = await linkReplaceService.ShortenLinksAsync(text);

    //var db = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
    //var links = await db.Links.ToListAsync();
    //var authors = await db.Authors.ToListAsync();

    // await dbInitializer.InitializeDbAsync(removeAtStart: false, useDataSeeder: false);
    // var service = scope.ServiceProvider.GetRequiredService<AddFileWithCategories>();
    // await service.AddFileToDatabase("test.txt");

    //Console.WriteLine("Get all files:");
    //var getAllFilesTest = scope.ServiceProvider.GetRequiredService<GetAllFilesTest>();
    //await getAllFilesTest.Get();

    //Console.WriteLine("Get file by Id:");
    //var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<SciMaterialsContext>>();
    //var allFiles = await unitOfWork.GetRepository<File>().GetAllAsync();
    //var fileId = allFiles.First().Id;
    //var getFileByIdTest = scope.ServiceProvider.GetRequiredService<GetFileByIdTest>();
    //await getFileByIdTest.Get(fileId);

    //var file = allFiles.First();
    //var editFileRequest = new EditFileRequest
    //{
    //    Id = file.Id,
    //    Categories = string.Join(",", file.Categories.Select(item => item.Id)) ?? "",
    //    Tags = string.Join(",", file.Tags.Select(item => item.Name)) ?? "",
    //    Description = "Тестовое описание",
    //    Title = "Тестовый заголовок"
    //};
    //var editFilesTest = scope.ServiceProvider.GetRequiredService<EditFilesTest>();
    //await editFilesTest.Edit(editFileRequest);
    //await getFileByIdTest.Get(file.Id);

    // var sendFileTest = scope.ServiceProvider.GetRequiredService<SendFileTest>();
    // await sendFileTest.SendFileAsync("test.txt");

    //var downloadFileByIdTest = scope.ServiceProvider.GetRequiredService<DownloadFileByIdTest>();
    //await downloadFileByIdTest.Download(Guid.Parse("29ED78FA-AE3E-4A5E-9981-F1816EE4E664"));
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
