using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.SqlLite.Auth.Migrations;

public static class Registrator
{
    public static void AddIdentitySQLite(this IServiceCollection services) =>
        services.AddDbContext<AuthDbContext>(
            (p, opt) => opt.UseSqlite(
                BuildConnectionString(p.GetRequiredService<IConfiguration>().GetSection("IdentityDatabase:ConnectionStrings:SQLite")),
                o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));

    /// <summary>
    /// Метод формирует строку подключения для БД SQLITE
    /// </summary>
    /// <param name="configuration">Конфигурация</param>
    /// <returns>Сформированную строку подключения</returns>
    public static string BuildConnectionString(IConfigurationSection Configuration)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = Configuration["datasource"]
        };

        var connectionString = builder.ConnectionString;
        return connectionString;
    }
}