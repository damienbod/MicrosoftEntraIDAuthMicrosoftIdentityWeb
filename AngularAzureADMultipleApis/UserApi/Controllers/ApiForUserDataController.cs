using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
