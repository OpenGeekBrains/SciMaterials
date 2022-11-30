using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using SciMaterials.Contracts.Identity.API;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.AUTH.Contracts;
using SciMaterials.DAL.AUTH.InitializationDb;
using SciMaterials.DAL.Contracts.Configuration;
using SciMaterials.MySql.Auth.Migrations;
using SciMaterials.Postgres.Auth.Migrations;
using SciMaterials.Services.Identity.API;
using SciMaterials.SqlLite.Auth.Migrations;
using SciMaterials.UI.MVC.Identity.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SciMaterials.UI.MVC;

public static class IdentityRegister
{
    /// <summary>Метод расширения по установке сервисов БД Identity</summary>
    /// <param name="Services">Сервисы</param>
    /// <param name="Configuration">Конфигурации</param>
    /// <returns>Коллекция сервисов</returns>
    /// <exception cref="Exception"></exception>
    public static IServiceCollection AddIdentityDatabase(this IServiceCollection Services, IConfiguration Configuration)
    {
        var providerName = Configuration.GetSection("IdentityDatabase").GetValue<string>(nameof(DatabaseSettings.Provider));

        switch (providerName.ToLower())
        {
            case "postgresql":
                Services.AddIdentityPostgres();
                break;
            case "mysql":
                Services.AddIdentityMySql();
                break;
            case "sqlite":
                Services.AddIdentitySQLite();
                break;
            default:
                throw new Exception($"Unsupported provider: {providerName}");
        }

        Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
        {
            opt.Password.RequiredLength = 5;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        Services.AddScoped<IdentityDatabaseManager>();

        return Services;
    }

    /// <summary>Метод расширения по установке сервисов для JWT и Swagger</summary>
    /// <param name="Services">Сервисы</param>
    /// <param name="Configuration">Конфигурации</param>
    /// <returns>Коллекция сервисов</returns>
    public static void ConfigureIdentityInSwagger(this SwaggerGenOptions Options)
    {
        Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });
        Options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    /// <summary>Метод расширения по установке утилит для работы с Identity</summary>
    /// <param name="Services">Сервисы</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddIdentityServices(this IServiceCollection Services, IConfiguration Configuration)
    {
        Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                        Configuration.GetValue<string>("IdentitySettings:SecretTokenKey"))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        Services.AddAuthorization();

        return Services.AddSingleton<IAuthUtils, AuthUtils>();
    }
    
    public static IServiceCollection AddIdentityClients(this IServiceCollection services, string serverUrl)
    {
        services.AddHttpClient<IIdentityApi, IdentityClient>("IdentityApi", c =>
        {
            c.BaseAddress = new Uri(serverUrl);
        });
        return services;
    }

    public static async Task InitializeIdentityDatabaseAsync(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var db_setting = configuration.GetSection("IdentityDatabase").Get<DatabaseSettings>();

        var manager = scope.ServiceProvider.GetRequiredService<IdentityDatabaseManager>();

        if (db_setting.RemoveAtStart) await manager.DeleteDatabaseAsync();

        await manager.InitializeDatabaseAsync();

        if (db_setting.UseDataSeeder) await manager.SeedDatabaseAsync();
    }
}