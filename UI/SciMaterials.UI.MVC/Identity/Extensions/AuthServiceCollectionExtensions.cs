using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SciMaterials.Contracts.Auth;
using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.AUTH.InitializationDb;
using SciMaterials.UI.MVC.Identity.Services;
using SciMaterials.WebApi.Clients.Identity;

namespace SciMaterials.UI.MVC.Identity.Extensions;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApiServices(this IServiceCollection Services, IConfiguration Configuration)
    {
        var provider = Configuration["AuthApiSettings:Provider"];
        
        Services.AddDbContext<AuthDbContext>(opt => _ = provider switch
        {
            "Sqlite" => opt.UseSqlite(AuthConnectionStrings.Sqlite(Configuration), OptionsBuilder =>
            {
                OptionsBuilder.MigrationsAssembly("SciMaterials.SqlLite.Auth.Migrations");
            }),
            "MySql" => opt.UseMySql(AuthConnectionStrings.MySql(Configuration), new MySqlServerVersion(new Version(8,0,30)), 
            OptionBuilder =>
            {
                OptionBuilder.MigrationsAssembly("SciMaterials.MySql.Auth.Migrations");
            }),
            _ => throw new Exception($"Unsupported provider: {provider}")
        });

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
        
        Services.AddHttpContextAccessor();
        
        return Services;
    }

    public static IServiceCollection AddAuthJwtAndSwaggerApiServices(this IServiceCollection Services, IConfiguration Configuration)
    {
        Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "SciMaterials",
                Version = "v1.1",
            });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                Name        = "Authorization",
                In          = ParameterLocation.Header,
                Type        = SecuritySchemeType.Http,
                Scheme      = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        });
        
        Services.AddAuthentication(x => 
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken            = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                    Configuration.GetSection("AuthApiSettings:SecretTokenKey:key").Value)),
                ValidateIssuer           = false,
                ValidateAudience         = false,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ClockSkew                = TimeSpan.Zero
            };
        });

        Services.AddAuthorization();
        
        return Services;
    }
    
    public static IServiceCollection AddAuthDbInitializer(this IServiceCollection Services)
    {
        return Services.AddScoped<IAuthDbInitializer, AuthDbInitializer>();
    }
    
    public static IServiceCollection AddAuthUtils(this IServiceCollection Services)
    {
        return Services.AddSingleton<IAuthUtilits, AuthUtils>();
    }

    public static IServiceCollection AddAuthHttpClient(this IServiceCollection Services)
    {
        Services
            .AddHttpClient()
            .AddSingleton<IIdentityClient, IdentityClient>();

        return Services;
    }
}