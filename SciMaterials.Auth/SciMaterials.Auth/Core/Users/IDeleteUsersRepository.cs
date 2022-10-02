using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Users;

public interface IDeleteUsersRepository
{
    Task<IdentityResult> DeleteUserByEmailAsync(string email);
}