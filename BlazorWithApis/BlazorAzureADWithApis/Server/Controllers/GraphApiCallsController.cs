using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAzureADWithApis.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorAzureADWithApis.Server.Controllers;

[Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "api://2b50a014-f353-4c10-aace-024f19a55569/access_as_user" })]
[ApiController]
[Route("[controller]")]
public class GraphApiCallsController : ControllerBase
{
    private GraphApiClientService _graphApiClientService;

    public GraphApiCallsController(GraphApiClientService graphApiClientService)
    {
        _graphApiClientService = graphApiClientService;
    }

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var userData = await _graphApiClientService.GetGraphApiUser();
        return new List<string> { $"DisplayName: {userData.DisplayName}",
            $"GivenName: {userData.GivenName}", $"AboutMe: {userData.AboutMe}" };
    }
}