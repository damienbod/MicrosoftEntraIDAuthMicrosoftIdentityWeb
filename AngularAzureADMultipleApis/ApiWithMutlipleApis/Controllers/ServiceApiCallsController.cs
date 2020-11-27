using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWithMutlipleApis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiWithMutlipleApis.Controllers
{
    [Authorize]
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
