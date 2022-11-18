using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.Contracts.WebApi.Clients.Comments;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.Contracts.WebApi.Clients.Tags;
using SciMaterials.Contracts.WebApi.Clients.Urls;
using SciMaterials.WebApi.Clients.Categories;
using SciMaterials.WebApi.Clients.Comments;
using SciMaterials.WebApi.Clients.ContentTypes;
using SciMaterials.WebApi.Clients.Files;
using SciMaterials.WebApi.Clients.Tags;
using SciMaterials.WebApi.Clients.Urls;

namespace SciMaterials.WebApi.Clients.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, Uri apiAddress)
    {
        services.AddHttpClient<IFilesClient, FilesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICategoriesClient, CategoriesClient>("CategoriesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICommentsClient, CommentsClient>("CommentClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<IContentTypesClient, ContentTypesClient>("ContentTypesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ITagsClient, TagsClient>("TagsClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<IUrlsClient, UrlsClient>("UrlsClient", c => c.BaseAddress = apiAddress);
        return services;
    }
}