using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorAzureADWithApis.Server.Controllers;

[Authorize(Policy = "ValidateAccessTokenPolicy",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "api://2b50a014-f353-4c10-aace-024f19a55569/access_as_user" })]
[ApiController]
[Route("[controller]")]
public class DirectApiController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new List<string> { "some data", "more data", "loads of data" };
    }
}