using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Auth.Core.Users;

public interface ILoginRepository
{
    Task<SignInResult> Login(string email, string password);
    Task<Task> Logout();
}