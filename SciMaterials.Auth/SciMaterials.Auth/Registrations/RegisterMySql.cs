using Microsoft.EntityFrameworkCore;
using SciMaterials.Auth.Data.Context;

namespace SciMaterials.Auth.Registrations;

public static class RegisterMySql
{
    public static IServiceCollection RegisterMySqlProvider(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<AuthDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("ConnectionString"),
                new MySqlServerVersion(new Version(8,0,30))));
    }
}