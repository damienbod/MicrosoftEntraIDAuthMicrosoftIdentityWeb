using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace WebApiWithRoles.Controllers
{
    [Authorize(Policy = "p-web-api-with-roles-admin")]
    [Authorize(Policy = "ValidateAccessTokenPolicy")]
    [AuthorizeForScopes(Scopes = new string[] { "api://5511b8d6-4652-4f2f-9643-59c61234e3c7/access_as_user" })]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new List<string> { "admin data 1", "admin data 2" });
        }
    }
}
