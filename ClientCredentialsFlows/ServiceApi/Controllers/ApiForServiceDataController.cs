using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ServiceApi.Controllers;

[Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Produces("application/json")]
[SwaggerTag("Service API for demo service data")]
public class ApiForServiceDataController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    [SwaggerOperation(OperationId = "Get", Summary = "Gets service data")]
    public IEnumerable<string> Get()
    {
        return new List<string> { "app-app Service API data 1", "service API data 2" };
    }
}
