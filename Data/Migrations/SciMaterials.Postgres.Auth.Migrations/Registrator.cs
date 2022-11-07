using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.Postgres.Auth.Migrations
{
    public static class Registrator
    {
        public static IServiceCollection AddAuthPostgresSqlProvider(this IServiceCollection Services, string ConnectionStrings)
        {
            Services.AddDbContext<AuthDbContext>(opt =>
            {
                opt.UseNpgsql(ConnectionStrings, OptionBuilder =>
                {
                    OptionBuilder.MigrationsAssembly(typeof(Registrator).Assembly.FullName);
                });
            });
            
            return Services;
        }
    }
}