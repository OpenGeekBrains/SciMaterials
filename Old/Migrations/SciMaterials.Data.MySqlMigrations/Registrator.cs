using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.Resources.Contexts;

namespace SciMaterials.Data.MySqlMigrations;

public static class Registrator
{
    public static void AddSciMaterialsContextMySql(this IServiceCollection services, string ConnectionString, int MajorVersion = 8, int MinorVersion = 0, int BuildVersion = 30) =>
        services.AddDbContext<SciMaterialsContext>(
            opt => opt.UseMySql(
                ConnectionString,
                new MySqlServerVersion(new Version(MajorVersion, MinorVersion, BuildVersion)),
                o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));
}
