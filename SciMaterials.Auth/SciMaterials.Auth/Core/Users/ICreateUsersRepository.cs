using Microsoft.AspNetCore.Identity;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Core.Users;

public interface ICreateUsersRepository
{
    Task<IdentityResult> CreateUserAsync(UserRequest user);
}