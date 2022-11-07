using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.MySql.Auth.Migrations
{
    public static class Registrator
    {
        public static IServiceCollection AddAuthMySqlProvider(
            this IServiceCollection Services,
            string ConnectionStrings, 
            int MajorVer = 8, int MinorVer = 0, int BuildVer = 30)
        {
            Services.AddDbContext<AuthDbContext>(opt =>
            {
                opt.UseMySql(ConnectionStrings, new MySqlServerVersion(new Version(MajorVer, MinorVer, BuildVer)), OptionBuilder =>
                {
                    OptionBuilder.MigrationsAssembly(typeof(Registrator).Assembly.FullName);
                });
            });
            
            return Services;
        }
    }
}