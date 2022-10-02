using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Roles;

public interface ICreateRolesRepository
{
    Task<IdentityResult> CreateRoleAsync(string roleName);
}