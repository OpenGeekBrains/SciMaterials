using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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
                
            Expires = DateTime.Now.AddMinutes(1),

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
            
        var security_token = jwt_security_token_handler.CreateToken(security_token_descriptor);

        return jwt_security_token_handler.WriteToken(security_token);
    }
    
    public bool CheckTokenIsEmptyOrInvalid(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return true;
        }

        var jwtToken = new JwtSecurityToken(token);
        var now = DateTime.UtcNow;
        return jwtToken is null 
            || jwtToken.ValidFrom >  now 
            || jwtToken.ValidTo < now;
    }
}