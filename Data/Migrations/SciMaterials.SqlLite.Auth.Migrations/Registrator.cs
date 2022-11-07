using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.SqlLite.Auth.Migrations
{
    public static class Registrator
    {
        public static IServiceCollection AddAuthSqliteProvider(this IServiceCollection Services, string ConnectionStrings)
        {
            Services.AddDbContext<AuthDbContext>(opt =>
            {
                opt.UseSqlite(ConnectionStrings, OptionBuilder =>
                {
                    OptionBuilder.MigrationsAssembly(typeof(Registrator).Assembly.FullName);
                });
            });
            
            return Services;
        }
    }
}