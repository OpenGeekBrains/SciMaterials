using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.Auth.Registrations;

public static class RegisterMySql
{
    public static IServiceCollection RegisterMySqlProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AuthDbConnection");

        var dbConfig = configuration.GetSection("DbConfig");
        
        var builder = new MySqlConnectionStringBuilder(connectionString);
        builder.Server = dbConfig["server"];
        builder.Database = dbConfig["database"];
        builder.Port = Convert.ToUInt32(dbConfig["port"]);
        builder.UserID = dbConfig["userid"];
        builder.Password = dbConfig["password"];

        connectionString = builder.ConnectionString;
        
        return services.AddDbContext<AuthDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8,0,30))));
    }
}