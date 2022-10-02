using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Roles;

public interface IReadAllRolesRepository
{
    Task<IReadOnlyList<IdentityRole>> GetAllRolesAsync();
}