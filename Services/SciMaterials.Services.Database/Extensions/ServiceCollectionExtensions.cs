using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Enums;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.DAL.Contexts;
using SciMaterials.Services.Database.Services.DbInitialization;

namespace SciMaterials.Services.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContextMultipleProviders(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var settings = configuration.GetSection("DbSettings")
            .Get<DbSettings>();

        services.AddDbContext<SciMaterialsContext>(options => _ = settings.DbProvider switch
        {
            nameof(DbProviders.SqlServer) => options.UseSqlServer(configuration.GetConnectionString("SqlServerConnectionString"),
                opt => opt.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
            nameof(DbProviders.PostgreSQL) => options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnectionString"),
                opt => opt.MigrationsAssembly("SciMaterials.PostgresqlMigrations")),
            nameof(DbProviders.MySQL) => options.UseMySql(configuration.GetConnectionString("MySQLConnectionString"), new MySqlServerVersion(new Version(8, 0, 30)),
                opt => opt.MigrationsAssembly("SciMaterials.Data.MySqlMigrations")),
            nameof(DbProviders.SQLite) => options.UseSqlite(configuration.GetConnectionString("SQLiteConnectionString"),
                opt => opt.MigrationsAssembly("SciMaterials.SQLiteMigrations")),
            _ => throw new Exception($"Unsupported provider: {settings.DbProvider}")
        });

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services) =>
        services.AddTransient<IDbInitializer, DbInitializer>();
}