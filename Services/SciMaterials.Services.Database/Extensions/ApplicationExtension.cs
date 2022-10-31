using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Auth;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.DAL.Contexts;

namespace SciMaterials.Services.Database.Extensions;

public static class ApplicationExtension
{
    public static async Task<IApplicationBuilder> InitializeDbAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var dbSetting = configuration.GetSection("DbSettings").Get<DbSettings>();
        
        if (dbSetting.DbProvider.Equals("SQLite"))
        {
            var context = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
            await context.Database.MigrateAsync().ConfigureAwait(false);
        }

        var authDb = scope.ServiceProvider.GetRequiredService<IAuthDbInitializer>();
        await authDb.InitializeAsync();

        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await initializer.InitializeDbAsync(
                RemoveAtStart: dbSetting.RemoveAtStart,
                UseDataSeeder: dbSetting.UseDataSeeder)
           .ConfigureAwait(false);

        return app;
    }
}