using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAzureADWithApis.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAzureADWithApis.Server.Controllers
{
    [Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
}
