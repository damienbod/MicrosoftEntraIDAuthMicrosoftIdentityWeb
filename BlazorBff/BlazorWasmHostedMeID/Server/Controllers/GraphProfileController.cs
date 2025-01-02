using BlazorWasmHostedMeID.Server.Services.Delegated;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorWasmHostedMeID.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "User.ReadBasic.All user.read" })]
[ApiController]
[Route("api/[controller]")]
public class GraphProfileController : ControllerBase
{
    private readonly MsGraphDelegatedService _microsoftGraphDelegatedClientService;

    public GraphProfileController(MsGraphDelegatedService microsoftGraphDelegatedClientService)
    {
        _microsoftGraphDelegatedClientService = microsoftGraphDelegatedClientService;
    }

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var userData = await _microsoftGraphDelegatedClientService.GetGraphApiUser();
        return new List<string> { $"DisplayName: {userData!.DisplayName}",
            $"GivenName: {userData.GivenName}", $"AboutMe: {userData.AboutMe}" };
    }
}
