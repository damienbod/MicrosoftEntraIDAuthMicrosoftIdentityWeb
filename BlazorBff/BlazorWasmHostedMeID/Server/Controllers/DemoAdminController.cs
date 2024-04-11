using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorWasmHostedMeID.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(Policy= "DemoAdmins", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "api://b2a09168-54e2-4bc4-af92-a710a64ef1fa/access_as_user" })]
[ApiController]
[Route("api/[controller]")]
public class DemoAdminController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new List<string> { "admin data", "secret admin record", "loads of admin data" };
    }
}
