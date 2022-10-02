using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Roles;

public interface IAddUserRole
{
    Task<IdentityResult> AddUserRole(string id, string roleName);
}