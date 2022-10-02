using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Roles;

public interface IReadRolesByIdRepository
{
    Task<IdentityRole> GetRoleByIdAsync(string id);
}