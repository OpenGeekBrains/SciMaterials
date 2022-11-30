using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.Postgres.Auth.Migrations;

public static class Registrator
{
    public static void AddIdentityPostgres(this IServiceCollection services) =>
        services.AddDbContext<AuthDbContext>(
            (p, opt) => opt.UseNpgsql(
                BuildConnectionString(p.GetRequiredService<IConfiguration>().GetSection("IdentityDatabase:ConnectionStrings:PostgresSQL")),
                o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));

    /// <summary>
    /// Метод формирует строку подключения для БД POSTGRESSQL
    /// </summary>
    /// <param name="Сonfiguration">Конфигурация</param>
    /// <returns>Сформированную строку подключения</returns>
    private static string BuildConnectionString(IConfigurationSection Сonfiguration)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Сonfiguration["host"],
            Port = Convert.ToInt32(Сonfiguration["port"]),
            Database = Сonfiguration["database"],
            Username = Сonfiguration["userid"],
            Password = Сonfiguration["password"]
        };

        var connectionString = builder.ConnectionString;

        return connectionString;
    }
}