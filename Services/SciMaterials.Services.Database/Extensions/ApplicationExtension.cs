using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Auth;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Initialization;

namespace SciMaterials.Services.Database.Extensions;

public static class ApplicationExtension
{
    public static async Task InitializeDbAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var authDb = scope.ServiceProvider.GetRequiredService<IAuthDbInitializer>();
        await authDb.InitializeAsync();

        var db_setting = configuration.GetSection("DbSettings").Get<DbSettings>();

        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await initializer.InitializeDbAsync(
                RemoveAtStart: db_setting.RemoveAtStart,
                UseDataSeeder: db_setting.UseDataSeeder);
    }
}