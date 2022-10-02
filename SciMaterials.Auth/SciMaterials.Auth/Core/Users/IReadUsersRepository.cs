using SciMaterials.Auth.Data.Models;

namespace SciMaterials.Auth.Core.Users;

public interface IReadUsersRepository
{
    Task<CustomIdentityUser> GetUserByEmailAsync(string email);
}