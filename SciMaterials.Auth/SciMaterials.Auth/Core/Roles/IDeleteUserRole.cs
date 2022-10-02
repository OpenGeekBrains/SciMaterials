using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Roles;

public interface IDeleteUserRole
{
    Task<IdentityResult> DeleteUserRole(string email, string roleName);
}