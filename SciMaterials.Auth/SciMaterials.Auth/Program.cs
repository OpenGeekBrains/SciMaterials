using Microsoft.AspNetCore.Identity;
using SciMaterials.Auth.Data;
using SciMaterials.Auth.Data.Context;
using SciMaterials.Auth.Data.Models;
using SciMaterials.Auth.Registrations;

namespace SciMaterials.Auth;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.RegisterMySqlProvider(builder.Configuration);
        builder.Services.AddIdentity<CustomIdentityUser, IdentityRole>(options =>
        {
            //По умолчанию поставил так
            options.Password.RequiredLength = 5; //минимальная длинна пароля
            options.Password.RequireNonAlphanumeric = false; //требуется ли применять символы
            options.Password.RequireLowercase = false; //требуются ли символы в нижнем регистре
            options.Password.RequireUppercase = false; //требуются ли символя в верхнем регистре
            options.Password.RequireDigit = false; //требуются ли применять цифры в пароле
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        
        builder.Services.AddHttpContextAccessor();

        builder.Services.RegisterAuthRepositories();

        var app = builder.Build();

        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        
        using (var scope = scopeFactory.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CustomIdentityUser>>();
            var rolesManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await RoleInitializer.InitializeAsync(userManager, rolesManager, builder.Configuration);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}