using System.Data.Common;

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
        var db_settings = configuration.GetSection("DbSettings");
        var dbSettings = db_settings
            .Get<DbSettings>();

        var providerName = dbSettings.GetProviderName();
        var connectionString = db_settings.GetConnectionString(dbSettings.DbProvider);

        var connection_settings = db_settings.GetSection("ConnectionSettings");

        var connection_string_builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
        foreach (var parameter in connection_settings.GetChildren())
            if(parameter.Value is { Length: > 0 } value)
            {
                connection_string_builder[parameter.Key] = value;
            }

        connectionString = connection_string_builder.ConnectionString;

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