using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public static class AuthDbInitializer
{
    /// <summary>
    /// Инициализация базы данных с созданием ролей "супер админ" и "пользователь"
    /// Создание одной учетной записи "админа"
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="roleManager"></param>
    public static async Task InitializeAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        var adminSettings = configuration.GetSection("AdminSettings");
        string adminEmail = adminSettings["login"];
        string adminPassword = adminSettings["password"];
        
        //Роль админа
        if (await roleManager.FindByNameAsync("admin") is null)
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
        }

        //Роль пользователя
        if (await roleManager.FindByNameAsync("user") is null)
        {
            await roleManager.CreateAsync(new IdentityRole("user"));
        }
        
        //Супер админ
        if (await userManager.FindByNameAsync(adminEmail) is null)
        {
            var superAdmin = new IdentityUser()
            {
                Email = adminEmail, 
                UserName = adminEmail
            };
            
            var identityResult = await userManager.CreateAsync(superAdmin, adminPassword);
                
            if (identityResult.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "admin");
            }
        }
    }
}