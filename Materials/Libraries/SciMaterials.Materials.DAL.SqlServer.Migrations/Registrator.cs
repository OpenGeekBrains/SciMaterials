using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.Resources.Contexts;

namespace SciMaterials.Materials.DAL.SqlServer.Migrations;

public static class Registrator
{
	public static void AddDbContextSqlServer(this IServiceCollection services, string connectionString) =>
		services.AddDbContext<SciMaterialsContext>(
			opt => opt.UseSqlServer(
				connectionString,
				o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));
}