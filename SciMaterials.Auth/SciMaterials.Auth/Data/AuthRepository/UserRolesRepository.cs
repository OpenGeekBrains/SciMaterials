using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Auth.Core.Roles;
using SciMaterials.Auth.Core.Users;
using SciMaterials.Auth.Data.Models;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Data.AuthRepository;

public interface IUserRolesRepository :
    ICreateRolesRepository, 
    IReadAllRolesRepository,
    IEditRolesRepository,
    IReadRolesByIdRepository,
    IDeleteRolesRepository,
    IAddUserRole,
    IDeleteUserRole,
    IListOfUserRoles
{ }

public class UserRolesRepository : IUserRolesRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<CustomIdentityUser> _userManager;
    private readonly ILogger<UserRolesRepository> _logger;
    public UserRolesRepository(
        RoleManager<IdentityRole> roleManager, 
        ILogger<UserRolesRepository> logger, 
        UserManager<CustomIdentityUser> userManager)
    {
        _roleManager = roleManager;
        _logger = logger;
        _userManager = userManager;
    }
    
    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        try
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }

    public async Task<IReadOnlyList<IdentityRole>> GetAllRolesAsync()
    {
        try
        {
            var result = await _roleManager.Roles.ToListAsync();
            if (result is not null)
            {
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }

    public async Task<IdentityResult> EditRoleByIdAsync(string id, RoleRequest roleToEdit)
    {
        try
        {
            var foundRole = await _roleManager.FindByIdAsync(id);
            
            foundRole.Name = roleToEdit.RoleName;
            
            var result = await _roleManager.UpdateAsync(foundRole);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }

    public async Task<IdentityRole> GetRoleByIdAsync(string id)
    {
        try
        {
            var result = await _roleManager.FindByIdAsync(id);
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

    public async Task<IdentityResult> DeleteRoleByIdAsync(string id)
    {
        try
        {
            var result = await _roleManager.FindByIdAsync(id);
            if (result is not null)
            {
                var userRole = await _roleManager.DeleteAsync(result);
                return userRole;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }

    public async Task<IdentityResult> AddUserRole(string email, string roleName)
    {
        try
        {
            var foundUser = await _userManager.FindByEmailAsync(email);
            var userRoles = await _userManager.GetRolesAsync(foundUser);
            
            var roles = await _roleManager.Roles.ToListAsync();
            if (!userRoles.Contains(roleName))
            {
                var isRoleContainsInSystem = roles.Select(x => x.Name.Contains(roleName));
                foreach (var isRole in isRoleContainsInSystem)
                {
                    if (isRole)
                    {
                        var roleAddedResult = await _userManager.AddToRoleAsync(foundUser, roleName);
                        if (roleAddedResult.Succeeded)
                        {
                            return roleAddedResult;
                        }
                    }
                }
                
                return null;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }

    public async Task<IdentityResult> DeleteUserRole(string email, string roleName)
    {
        try
        {
            var foundUser = await _userManager.FindByEmailAsync(email);
            var userRoles = await _userManager.GetRolesAsync(foundUser);
            
            var roles = await _roleManager.Roles.ToListAsync();
            if (userRoles.Contains(roleName))
            {
                var isRoleContainsInSystem = roles.Select(x => x.Name.Contains(roleName));
                foreach (var isRole in isRoleContainsInSystem)
                {
                    if (isRole)
                    {
                        var roleRemovedResult = await _userManager.RemoveFromRoleAsync(foundUser, roleName);
                        if (roleRemovedResult.Succeeded)
                        {
                            return roleRemovedResult;
                        }
                    }
                }
                
                return null;
            }

            return null;
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }

    public async Task<IReadOnlyList<string>> ListOfUserRoles(string email)
    {
        try
        {
            var foundUser = await _userManager.FindByEmailAsync(email);
            if (foundUser is not null)
            {
                var userRoles = await _userManager.GetRolesAsync(foundUser);

                if (userRoles is not null)
                {
                    return userRoles.ToList();
                }

                return null;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }

        return null;
    }
}