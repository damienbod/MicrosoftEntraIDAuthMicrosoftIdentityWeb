using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace WebApiWithRoles.Controllers
{
    [Authorize(Policy = "p-web-api-with-roles-student")]
    [Authorize(Policy = "ValidateAccessTokenPolicy")]
    [AuthorizeForScopes(Scopes = new string[] { "api://5511b8d6-4652-4f2f-9643-59c61234e3c7/access_as_user" })]
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
