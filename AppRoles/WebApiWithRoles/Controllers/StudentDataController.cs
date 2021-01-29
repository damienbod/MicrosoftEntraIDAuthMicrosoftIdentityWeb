using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWithRoles.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StudentDataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new List<string> { "student data 1", "student data 2" });
        }
    }
}
