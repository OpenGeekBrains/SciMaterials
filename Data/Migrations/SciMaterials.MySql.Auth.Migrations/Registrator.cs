using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MySqlConnector;

using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.MySql.Auth.Migrations;

public static class Registrator
{
    public static void AddIdentityMySql(this IServiceCollection services) =>
        services.AddDbContext<AuthDbContext>(
            (p, opt) =>
            {
                var mySqlConfigurationSection = p.GetRequiredService<IConfiguration>().GetSection("IdentityDatabase:ConnectionStrings:MySQL");

                opt.UseMySql(
                    BuildConnectionString(mySqlConfigurationSection),
                    new MySqlServerVersion(new Version(GetMySqlVersion(mySqlConfigurationSection))),
                    o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName));
            });

    /// <summary>
    /// Метод формирует строку подключения для БД MYSQL
    /// </summary>
    /// <param name="configuration">Конфигурация</param>
    /// <returns>Сформированную строку подключения</returns>
    private static string BuildConnectionString(IConfigurationSection configuration)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = configuration["server"],
            Port = Convert.ToUInt32(configuration["port"]),
            Database = configuration["database"],
            UserID = configuration["userid"],
            Password = configuration["password"]
        };

        var connectionString = builder.ConnectionString;
        return connectionString;
    }

    private static string GetMySqlVersion(IConfigurationSection configuration)
    {
        return configuration["version"];
    }
}