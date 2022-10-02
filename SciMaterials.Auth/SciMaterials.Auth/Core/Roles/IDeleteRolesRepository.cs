using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Roles;

public interface IDeleteRolesRepository
{
    Task<IdentityResult> DeleteRoleByIdAsync(string id);
}