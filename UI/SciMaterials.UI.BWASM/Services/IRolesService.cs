using Microsoft.AspNetCore.Identity;

namespace SciMaterials.UI.BWASM.Services;

public interface IRolesService
{
    Task<bool> AddRole(string roleName);
    Task<bool> DeleteRole(string roleId);
    Task<List<IdentityRole>> RolesList();
}