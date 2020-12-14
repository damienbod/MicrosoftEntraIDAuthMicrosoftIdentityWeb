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
    public class ServiceApiCallsController : ControllerBase
    {
        private ServiceApiClientService _serviceApiClientService;

        public ServiceApiCallsController(ServiceApiClientService serviceApiClientService)
        {
            _serviceApiClientService = serviceApiClientService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await _serviceApiClientService.GetApiDataAsync();
        }
    }
}
