using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Roles;

public interface IListOfUserRoles
{
    Task<IReadOnlyList<string>> ListOfUserRoles(string email);
}