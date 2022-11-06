using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.Contracts.WebApi.Clients.Comments;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.Contracts.WebApi.Clients.Tags;
using SciMaterials.WebApi.Clients.Categories;
using SciMaterials.WebApi.Clients.Comments;
using SciMaterials.WebApi.Clients.ContentTypes;
using SciMaterials.WebApi.Clients.Files;
using SciMaterials.WebApi.Clients.Tags;

namespace SciMaterials.WebApi.Clients.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, Uri apiAddress)
    {
        services.AddHttpClient<IFilesClient, FilesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICategoriesClient, CategoriesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICommentsClient, CommentsClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<IContentTypesClient, ContentTypesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ITagsClient, TagsClient>("FilesClient", c => c.BaseAddress = apiAddress);
        return services;
    }
}