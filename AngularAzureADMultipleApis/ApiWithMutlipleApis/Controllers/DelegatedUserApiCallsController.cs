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
    public class DelegatedUserApiCallsController : ControllerBase
    {
        private UserApiClientService _userApiClientService;

        public DelegatedUserApiCallsController(UserApiClientService userApiClientService)
        {
            _userApiClientService = userApiClientService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await _userApiClientService.GetApiDataAsync();
        }
    }
}
