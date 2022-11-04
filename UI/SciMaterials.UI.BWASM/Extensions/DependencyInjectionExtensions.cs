// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiClient<TClient, TImplementation>(
        this IServiceCollection services,
        string routeBase,
        Action<IHttpClientBuilder>? clientConfiguration = null)
        where TClient : class
        where TImplementation : class, TClient
    {
        var builder = services.AddHttpClient<TClient, TImplementation>(c => c.BaseAddress = new Uri(routeBase));
        clientConfiguration?.Invoke(builder);
        return services;
    }
}