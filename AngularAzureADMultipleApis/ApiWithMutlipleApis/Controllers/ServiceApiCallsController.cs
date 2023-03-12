using ApiWithMutlipleApis.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace ApiWithMutlipleApis.Controllers;

[Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "api://2b50a014-f353-4c10-aace-024f19a55569/access_as_user" })]
[ApiController]
[Route("[controller]")]
public class ServiceApiCallsController : ControllerBase
{
    private ServiceApiClientService _serviceApiClientService;

    public ServiceApiCallsController(ServiceApiClientService serviceApiClientService)
    {
        _serviceApiClientService = serviceApiClientService;
    }

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        return await _serviceApiClientService.GetApiDataAsync();
    }
}
