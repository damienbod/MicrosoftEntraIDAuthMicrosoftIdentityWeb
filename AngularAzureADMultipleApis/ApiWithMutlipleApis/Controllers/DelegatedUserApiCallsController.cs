using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiWithMutlipleApis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DelegatedUserApiCallsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string> { "some data", "more data", "loads of data" };
        }
    }
}
