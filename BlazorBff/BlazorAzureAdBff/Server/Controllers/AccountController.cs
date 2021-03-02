using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAzureADWithApis.Server.Controllers
{
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpGet("Login")]
        public ActionResult Login(string returnUrl)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/" });
        }

        [HttpGet("Logout")]
        public IActionResult Logout() => SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
