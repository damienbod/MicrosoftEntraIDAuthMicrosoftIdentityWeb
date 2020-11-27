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
            return new List<string> { userData.AboutMe, userData.DisplayName, userData.City };
        }
    }
}
