using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public interface IRolesService
{
    Task<bool> AddRole(string roleName);
    Task<bool> DeleteRole(string roleId);
    Task<IReadOnlyList<UserRole>> RolesList();
}