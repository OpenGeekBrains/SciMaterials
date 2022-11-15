using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.Contexts;

namespace SciMaterials.PostgresqlMigrations;

public static class Registrator
{
    public static IServiceCollection AddSciMaterialsContextPostgreSQL(this IServiceCollection services, string connectionString)
    {
        AppContext.SetSwitch(switchName: "Npgsql.EnableLegacyTimestampBehavior", isEnabled: true);

        services.AddDbContext<SciMaterialsContext>(
            opt => opt.UseNpgsql(connectionString,
                o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));

        return services;
    }
}
