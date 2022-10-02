using Microsoft.AspNetCore.Identity;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Core.Users;

public interface IEditUsersRepository
{
    Task<IdentityResult> EditUserByEmailAsync(string email, UserRequest user);
}