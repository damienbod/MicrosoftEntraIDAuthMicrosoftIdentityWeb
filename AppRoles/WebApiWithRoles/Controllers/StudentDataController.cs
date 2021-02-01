using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWithRoles.Controllers
{
    [Authorize(Policy = "p-web-api-with-roles-student")]
    [Authorize(Policy = "ValidateAccessTokenPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentDataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new List<string> { "student data 1", "student data 2" });
        }
    }
}
