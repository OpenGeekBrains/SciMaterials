using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// ReSharper disable once CheckNamespace
namespace System;

public static class JwtStringToClaimsExtension
{
    public static IReadOnlyCollection<Claim> ParseJwt(this string jwt)
    {
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims.ToList();
        if (claims.Where(x => x.Type == "role").ToArray() is {Length: > 0 } roleClaims)
        {
            foreach (Claim roleClaim in roleClaims)
            {
                var index = claims.IndexOf(roleClaim);
                claims[index] = new Claim(ClaimTypes.Role, roleClaim.Value);
            }
        }
        return claims;
    }
}