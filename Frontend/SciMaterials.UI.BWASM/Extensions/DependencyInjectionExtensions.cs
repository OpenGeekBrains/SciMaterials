using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using SciMaterials.UI.BWASM.Extensions;

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
        var builder = services.AddHttpClient<TClient, TImplementation>(c => c.BaseAddress = new Uri(routeBase))
           .SetHandlerLifetime(TimeSpan.FromMinutes(5))
           .AddPolicyHandler(HttpPoliciesExtension.GetRetryPolicy());
        clientConfiguration?.Invoke(builder);
        return services;
    }
    
    public static IServiceCollection AddApiClient<TClient>(
        this IServiceCollection services,
        string routeBase,
        Action<IHttpClientBuilder>? clientConfiguration = null)
        where TClient : class
    {
        services.TryAddScoped<TClient>();
        var builder = services.AddHttpClient<TClient>(c => c.BaseAddress = new Uri(routeBase));
        clientConfiguration?.Invoke(builder);
        return services;
    }
}