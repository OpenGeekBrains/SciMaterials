using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.Data.MySqlMigrations;
using SciMaterials.MsSqlServerMigrations;
using SciMaterials.PostgresqlMigrations;
using SciMaterials.Services.Database.Services.DbInitialization;
using SciMaterials.SQLiteMigrations;

namespace SciMaterials.Services.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseProviders(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DbSettings")
            .Get<DbSettings>();

        var providerName = dbSettings.GetProviderName();
        var connectionString = configuration.GetSection("DbSettings").GetConnectionString(dbSettings.DbProvider);

        switch (providerName.ToLower())
        {
            case "sqlserver":
                services.AddSciMaterialsContextSqlServer(connectionString);
                break;
            case "postgresql":
                services.AddSciMaterialsContextPostgreSQL(connectionString);
                break;
            case "mysql":
                services.AddSciMaterialsContextMySql(connectionString);
                break;
            case "sqlite":
                services.AddSciMaterialsContextSqlite(connectionString);
                break;
            default:
                throw new Exception($"Unsupported provider: {providerName}");
        }

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services) =>
       services.AddTransient<IDbInitializer, DbInitializer>();
}