using SciMaterials.Auth.Data.Models;

namespace SciMaterials.Auth.Core.Users;

public interface IReadAllUsersRepository
{
    Task<IReadOnlyList<CustomIdentityUser>> GetAllUsersAsync();
}