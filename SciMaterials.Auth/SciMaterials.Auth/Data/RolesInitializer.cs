using Microsoft.AspNetCore.Identity;
using SciMaterials.Auth.Data.Models;

namespace SciMaterials.Auth.Data;

public static class RoleInitializer
{
    /// <summary>
    /// Инициализация базы данных с созданием ролей "супер админ" и "пользователь"
    /// Создание одной учетной записи "админа"
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="roleManager"></param>
    public static async Task InitializeAsync(
        UserManager<CustomIdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration)
    {
        string superAdminEmail = configuration.GetSection("SuperAdminSettings:Login").Value;
        string superAdminPassword = configuration.GetSection("SuperAdminSettings:Password").Value;
        
        //Роль супер админа
        if (await roleManager.FindByNameAsync("sa") is null)
        {
            await roleManager.CreateAsync(new IdentityRole("sa"));
        }

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
        if (await userManager.FindByNameAsync(superAdminEmail) is null)
        {
            var superAdmin = new CustomIdentityUser
            {
                Email = superAdminEmail, 
                UserName = superAdminEmail
            };
            
            var identityResult = await userManager.CreateAsync(superAdmin, superAdminPassword);
                
            if (identityResult.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "sa");
            }
        }
    }
}