using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeviceFlowWeb;

public class AuthenticationSignInService
{
    public async Task SignIn(HttpContext httpContext, 
        string accessToken, string idToken, int expiresIn)
    {
        var claims = GetClaims(idToken);

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            "name",
            "user");

        var authProperties = new AuthenticationProperties();
        authProperties.ExpiresUtc = DateTime.UtcNow.AddSeconds(expiresIn);

        // save the tokens in the cookie
        authProperties.StoreTokens(new List<AuthenticationToken>
        {
            new AuthenticationToken
            {
                Name = "access_token",
                Value = accessToken
            },
            new AuthenticationToken
            {
                Name = "id_token",
                Value = idToken
            }
        });

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), 
            authProperties);
    }

    private IEnumerable<Claim> GetClaims(string token)
    {
        var validJwt = new JwtSecurityToken(token);
        return validJwt.Claims;
    }
}