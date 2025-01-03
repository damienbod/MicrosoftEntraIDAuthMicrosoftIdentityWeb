﻿using ApiWithMutlipleApis.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace ApiWithMutlipleApis.Controllers;

[Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = ["User.ReadBasic.All", "user.read"])]
[ApiController]
[Route("[controller]")]
public class GraphApiCallsController : ControllerBase
{
    private readonly GraphApiClientService _graphApiClientService;

    public GraphApiCallsController(GraphApiClientService graphApiClientService)
    {
        _graphApiClientService = graphApiClientService;
    }

    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        var userData = await _graphApiClientService.GetGraphApiUser();

        return [ $"DisplayName: {userData!.DisplayName}",
            $"GivenName: {userData!.GivenName}", $"AboutMe: {userData!.AboutMe}" ];
    }
}
