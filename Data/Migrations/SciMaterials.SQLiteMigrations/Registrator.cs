using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.Contexts;

namespace SciMaterials.SQLiteMigrations;

public static class Registrator
{
    public static IServiceCollection AddSciMaterialsContextSqlite(this IServiceCollection services, string connectionString) =>
        services.AddDbContext<SciMaterialsContext>(
            opt => opt.UseSqlite(connectionString,
                o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));
}
