using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using Newtonsoft.Json.Linq;

namespace UserApiOne.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApiForUserDataController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string> { "user API data 1", "user API data 1" };
        }
    }
}
