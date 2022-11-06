using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Auth;

namespace SciMaterials.UI.MVC.Identity.Services;

/// <summary>
/// Утилиты по работе с jwt токенами
/// </summary>
public class AuthUtils : IAuthUtilits
{
    private readonly IConfiguration _Configuration;
    private string _SecretKey;

    public AuthUtils(IConfiguration configuration)
    {
        _Configuration = configuration;
    }
    
    /// <summary>
    /// Метод создает jwt токен сессии
    /// </summary>
    /// <param name="User">Пользователь</param>
    /// <param name="Roles">Список ролей в системе</param>
    /// <returns>Возращает токен</returns>
    public string CreateSessionToken(IdentityUser User, IList<string> Roles)
    {
        var config = _Configuration.GetSection("AuthApiSettings:SecretTokenKey");
        _SecretKey = config["key"];
        
        var jwt_security_token_handler = new JwtSecurityTokenHandler();
        
        var key = Encoding.ASCII.GetBytes(_SecretKey);

        //Claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, User.Id),
            new(ClaimTypes.Name, User.UserName),
            new(ClaimTypes.Email, User.Email)
        };

        claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var security_token_descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
                
            Expires = DateTime.Now.AddMinutes(15),

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
            
        var security_token = jwt_security_token_handler.CreateToken(security_token_descriptor);

        return jwt_security_token_handler.WriteToken(security_token);
    }

    /// <summary>
    /// Метод проверки ролей админа и пользователя, которые удалять нельзя
    /// </summary>
    /// <param name="Role">Роль</param>
    /// <returns></returns>
    public bool CheckToDeleteAdminOrUserRoles(IdentityRole Role)
    {
        if (Role.Name.Equals(AuthApiRoles.Admin) || Role.Name.Equals(AuthApiRoles.User)) return false;

        return true;
    }

    /// <summary>
    /// Метод проверки супер админа в роли админа, которого удалять нельзя
    /// </summary>
    /// <param name="User">Пользователь</param>
    /// <param name="RoleName">Название роли</param>
    /// <returns></returns>
    public bool CheckToDeleteSAInRoleAdmin(IdentityUser User, string RoleName)
    {
        if (User.Email.Equals(_Configuration.GetSection("AuthApiSettings:AdminSettings:login").Value) &&
            RoleName.Equals(AuthApiRoles.Admin)) return false;

        return true;
    }

    /// <summary>
    /// Метод проверки супер админа, которого удалять нельзя
    /// </summary>
    /// <param name="User">Пользователь</param>
    /// <returns></returns>
    public bool CheckToDeleteSA(IdentityUser User)
    {
        if (User.Email.Equals(_Configuration.GetSection("AuthApiSettings:AdminSettings:login").Value)) return false;

        return true;
    }
}