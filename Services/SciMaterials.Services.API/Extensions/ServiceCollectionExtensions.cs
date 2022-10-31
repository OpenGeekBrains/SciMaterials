using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.API.Extensions;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.Services.Authors;
using SciMaterials.Services.API.Services.Categories;
using SciMaterials.Services.API.Services.Files;
using SciMaterials.Services.API.Services.Files.Stores;
using SciMaterials.Services.API.Services.Authors;
using SciMaterials.Contracts.API.Services.Comments;
using SciMaterials.Services.API.Services.Comments;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Services.API.Services.ContentTypes;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.RepositoryLib.Extensions;
using SciMaterials.Services.API.Services.Tags;
using SciMaterials.Services.Database.Extensions;
using SciMaterials.Contracts.API.Services.Urls;
using SciMaterials.Services.API.Services.Urls;

namespace SciMaterials.Services.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileStore, FileSystemStore>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IContentTypeService, ContentTypeService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUrlService, UrlService>();
        services.AddRepositoryServices();

        services
           .AddContextMultipleProviders(configuration)
           .AddDatabaseServices();

        services.AddMappings();
        return services;
    }
}