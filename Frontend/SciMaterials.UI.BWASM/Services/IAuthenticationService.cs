using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public interface IAuthenticationService
{
     Task Logout();
     Task<bool> SignIn(SignInForm formData);
     Task<bool> SignUp(SignUpForm formData);
     Task<bool> IsCurrentUser(string userEmail);
     Task RefreshCurrentUser();
}