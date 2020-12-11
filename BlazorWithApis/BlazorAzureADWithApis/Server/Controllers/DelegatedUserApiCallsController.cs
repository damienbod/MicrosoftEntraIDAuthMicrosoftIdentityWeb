using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAzureADWithApis.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace BlazorAzureADWithApis.Server.Controllers
{
    [Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class DelegatedUserApiCallsController : ControllerBase
    {
        private UserApiClientService _userApiClientService;
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public DelegatedUserApiCallsController(UserApiClientService userApiClientService)
        {
            _userApiClientService = userApiClientService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            return await _userApiClientService.GetApiDataAsync();
        }
    }
}
