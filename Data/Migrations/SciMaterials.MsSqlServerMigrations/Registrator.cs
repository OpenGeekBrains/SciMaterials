using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.Contexts;

namespace SciMaterials.MsSqlServerMigrations;

public static class Registrator
{
    public static IServiceCollection AddSciMaterialsContextSqlServer(this IServiceCollection services, string connectionString) =>
        services.AddDbContext<SciMaterialsContext>(
            opt => opt.UseSqlServer(
                connectionString,
                o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));
}
