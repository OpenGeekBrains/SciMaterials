using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

using SciMaterials.Contracts.API.Mapping;
using SciMaterials.Contracts.API.Services.Authors;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.Services.Comments;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.Services.Resources;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.API.Services.Urls;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.Contracts.WebApi.Clients.Comments;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.Contracts.WebApi.Clients.Tags;
using SciMaterials.Contracts.WebApi.Clients.Urls;
using SciMaterials.DAL.Contracts.Configuration;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Ratings;
using SciMaterials.DAL.Resources.Repositories.Users;
using SciMaterials.DAL.Resources.Services;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.Materials.Api.Filters;
using SciMaterials.Materials.DAL.SqlServer.Migrations;
using SciMaterials.Services.API.Services.Authors;
using SciMaterials.Services.API.Services.Categories;
using SciMaterials.Services.API.Services.Comments;
using SciMaterials.Services.API.Services.ContentTypes;
using SciMaterials.Services.API.Services.Files;
using SciMaterials.Services.API.Services.Files.Stores;
using SciMaterials.Services.API.Services.Resources;
using SciMaterials.Services.API.Services.Tags;
using SciMaterials.Services.API.Services.Urls;
using SciMaterials.UrlsService.Contracts;
using SciMaterials.UrlsService.Contracts.Settings;
using SciMaterials.UrlsService.Domain;
using SciMaterials.WebApi.Clients.Categories;
using SciMaterials.WebApi.Clients.Comments;
using SciMaterials.WebApi.Clients.ContentTypes;
using SciMaterials.WebApi.Clients.Files;
using SciMaterials.WebApi.Clients.Tags;
using SciMaterials.WebApi.Clients.Urls;

using Swashbuckle.AspNetCore.SwaggerGen;

using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.Materials.Api;

public static class ResourcesRegister
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, string serverUrl)
    {
        Uri apiAddress = new Uri(serverUrl);
        services.AddHttpClient<IFilesClient, FilesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICategoriesClient, CategoriesClient>("CategoriesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICommentsClient, CommentsClient>("CommentClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<IContentTypesClient, ContentTypesClient>("ContentTypesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ITagsClient, TagsClient>("TagsClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<IUrlsClient, UrlsClient>("UrlsClient", c => c.BaseAddress = apiAddress);
        return services;
    }

    public static IServiceCollection AddResourcesDatabaseProviders(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();

        var providerName = dbSettings.GetProviderName();
        var connectionString = configuration.GetConnectionString(dbSettings.Provider);

        switch (providerName.ToLower())
        {
            case "sqlserver":
                services.AddDbContextSqlServer(connectionString);
                break;
            default:
                throw new Exception($"Unsupported provider: {providerName}");
        }

        return services;
    }

    public static async Task InitializeResourcesDatabaseAsync(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var db_setting = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();

        var manager = scope.ServiceProvider.GetRequiredService<ResourcesDatabaseManager>();

        if (db_setting.RemoveAtStart) await manager.DeleteDatabaseAsync();

        await manager.InitializeDatabaseAsync();

        if (db_setting.UseDataSeeder) await manager.SeedDatabaseAsync();
    }

    public static IServiceCollection AddResourcesDataLayer(this IServiceCollection services)
    {
        services
           .AddScoped<IRepository<Category>, CategoryRepository>()
           .AddScoped<IRepository<Comment>, CommentRepository>()
           .AddScoped<IRepository<ContentType>, ContentTypeRepository>()
           .AddScoped<IRepository<FileGroup>, FileGroupRepository>()
           .AddScoped<IRepository<File>, FileRepository>()
           .AddScoped<IRepository<Tag>, TagRepository>()
           .AddScoped<IRepository<Url>, UrlRepository>()
           .AddScoped<IRepository<Rating>, RatingRepository>()
           .AddScoped<IRepository<Author>, AuthorRepository>()
           .AddScoped<IRepository<User>, UserRepository>()
           .AddScoped<IRepository<Resource>, ResourceRepository>();

        services
            .AddScoped<ResourcesDatabaseManager>()
            .AddScoped<IUnitOfWork<SciMaterialsContext>, SciMaterialsFilesUnitOfWork>();

        return services;
    }

    public static IServiceCollection AddResourcesApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMappings();

        services
            .Configure<LinkShortCutOptions>(configuration.GetSection(LinkShortCutOptions.SectionName))
            .AddScoped<IUrlShortenerClient, UrlShortenerClient>()
            .AddScoped<ILinkReplaceService, LinkReplaceService>();

        services
            .AddScoped<IFileStore, FileSystemStore>()
            .AddScoped<IFileService, FileService>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IContentTypeService, ContentTypeService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUrlService, UrlService>();
        services.AddScoped<IResourceService, ResourceService>();

        return services;
    }

    public static void AddFileUploadFilter(this SwaggerGenOptions options)
    {
        options.OperationFilter<FileUploadFilter>();
    }

    public static void AddOptionalRouteParameterOperationFilter(this SwaggerGenOptions options)
    {
        options.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();
    }

    public static IServiceCollection ConfigureFilesUploadSupport(this IServiceCollection services, IConfiguration config)
    {
        var api_settings = config.GetSection("FilesApiSettings");

        services
            .Configure<ApiSettings>(api_settings)
            .AddSingleton<ApiSettings>(s => s.GetRequiredService<IOptions<ApiSettings>>().Value);

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = api_settings.GetValue<long>("MaxFileSize");
        });

        return services;
    }
}