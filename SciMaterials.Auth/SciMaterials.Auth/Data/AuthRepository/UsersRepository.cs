using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Auth.Core.Users;
using SciMaterials.Auth.Data.Models;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Data.AuthRepository;

public interface IUsersRepository : 
    ICreateUsersRepository, 
    IReadUsersRepository, 
    IReadAllUsersRepository, 
    IDeleteUsersRepository,
    IEditUsersRepository,
    ILoginRepository
{ }

public class UsersRepository : IUsersRepository
{
    private readonly UserManager<CustomIdentityUser> _userManager;
    private readonly SignInManager<CustomIdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(
        UserManager<CustomIdentityUser> userManager,
        ILogger<UsersRepository> logger, 
        SignInManager<CustomIdentityUser> signInManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _logger = logger;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }
    
    public async Task<IdentityResult> CreateUserAsync(UserRequest createUser)
    {
        try
        {
            var user = new CustomIdentityUser()
            {
                Email = createUser.Email,
                UserName = createUser.Email,
                PhoneNumber = createUser.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, createUser.Password);
            await _userManager.AddToRoleAsync(user, "user");
            await _signInManager.SignInAsync(user, false);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }
    public async Task<IReadOnlyList<CustomIdentityUser>> GetAllUsersAsync()
    {
        try
        {
            var listOfAllUsers = await _userManager.Users.ToListAsync();
            return listOfAllUsers;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }
    public async Task<CustomIdentityUser> GetUserByEmailAsync(string email)
    {
        try
        {
            var result = await _userManager.FindByEmailAsync(email);
            if (result is not null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }
        
        return null;
    }
    public async Task<IdentityResult> DeleteUserByEmailAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.DeleteAsync(user);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }
    public async Task<IdentityResult> EditUserByEmailAsync(string email, UserRequest user)
    {
        try
        {
            var foundUser = await _userManager.FindByEmailAsync(email);
            if (foundUser is not null)
            {
                foundUser.Email = user.Email;
                foundUser.PhoneNumber = user.PhoneNumber;
                foundUser.UserName = user.Email;

                var result = await _userManager.UpdateAsync(foundUser);
                return result;
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }

    public async Task<SignInResult> Login(string email, string password)
    {
        var identityResult = await _userManager.FindByEmailAsync(email);
        if (identityResult is not null)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(
                userName: email, 
                password: password, 
                isPersistent: true, 
                lockoutOnFailure: false);
            
            if (signInResult.Succeeded)
            {
                return signInResult;
            }

            return new SignInResult();
        }

        return new SignInResult();
    }

    public async Task<Task> Logout()
    {
        await _signInManager.SignOutAsync();
        return Task.CompletedTask;
    }
}