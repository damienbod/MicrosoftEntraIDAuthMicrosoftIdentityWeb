using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAzureADWithApis.Server.Services.Delegated;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorAzureADWithApis.Server.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [AuthorizeForScopes(Scopes = new string[] { "User.ReadBasic.All user.read" })]
    [ApiController]
    [Route("api/[controller]")]
    public class GraphApiCallsController : ControllerBase
    {
        private MicrosoftGraphDelegatedClientService _microsoftGraphDelegatedClientService;

        public GraphApiCallsController(MicrosoftGraphDelegatedClientService microsoftGraphDelegatedClientService)
        {
            _microsoftGraphDelegatedClientService = microsoftGraphDelegatedClientService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var userData = await _microsoftGraphDelegatedClientService.GetGraphApiUser();
            return new List<string> { $"DisplayName: {userData.DisplayName}",
                $"GivenName: {userData.GivenName}", $"AboutMe: {userData.AboutMe}" };
        }
    }
}
