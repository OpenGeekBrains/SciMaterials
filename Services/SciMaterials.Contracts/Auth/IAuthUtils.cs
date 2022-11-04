namespace SciMaterials.Contracts.Auth;

public interface IAuthUtils<in TUser>
{
    string CreateSessionToken(TUser User, IList<string> Roles);
    bool CheckTokenIsEmptyOrInvalid(string token);
}