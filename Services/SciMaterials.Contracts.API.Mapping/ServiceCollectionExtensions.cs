using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace SciMaterials.Contracts.API.Mapping;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}