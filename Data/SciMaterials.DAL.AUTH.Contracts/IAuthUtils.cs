using Microsoft.AspNetCore.Identity;

namespace SciMaterials.DAL.AUTH.Contracts;

public interface IAuthUtils
{
    string CreateSessionToken(IdentityUser User, IList<string> Roles);
    bool CheckToDeleteAdminOrUserRoles(IdentityRole Role);
    bool CheckToDeleteSAInRoleAdmin(IdentityUser User, string Role);
    bool CheckToDeleteSA(IdentityUser User);
}