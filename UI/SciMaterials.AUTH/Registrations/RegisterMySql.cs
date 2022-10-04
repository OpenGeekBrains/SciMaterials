using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.Contexts;

namespace SciMaterials.Auth.Registrations;

public static class RegisterMySql
{
    public static IServiceCollection RegisterMySqlProvider(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<SciMaterialsAuthDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("ConnectionString"),
                new MySqlServerVersion(new Version(8,0,30))));
    }
}