using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.DAL.SqlServer.Migrations;

public static class Registrator
{
	public static void AddDbContextSqlServer(this IServiceCollection services, string connectionString) =>
		services.AddDbContext<AuthDbContext>(
			opt => opt.UseSqlServer(
				connectionString,
				o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));
}