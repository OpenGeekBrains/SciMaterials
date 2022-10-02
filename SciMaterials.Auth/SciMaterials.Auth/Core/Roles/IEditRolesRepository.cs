using Microsoft.AspNetCore.Identity;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Core.Roles;

public interface IEditRolesRepository
{
    Task<IdentityResult> EditRoleByIdAsync(string id, RoleRequest roleToEdit);
}