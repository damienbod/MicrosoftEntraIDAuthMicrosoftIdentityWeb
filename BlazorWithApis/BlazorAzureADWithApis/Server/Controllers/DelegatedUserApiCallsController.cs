﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAzureADWithApis.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace BlazorAzureADWithApis.Server.Controllers
{
    [Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AuthorizeForScopes(Scopes = new string[] { "api://2b50a014-f353-4c10-aace-024f19a55569/access_as_user" })]
    [ApiController]
    [Route("[controller]")]
    public class DelegatedUserApiCallsController : ControllerBase
    {
        private UserApiClientService _userApiClientService;
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public DelegatedUserApiCallsController(UserApiClientService userApiClientService)
        {
            _userApiClientService = userApiClientService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await _userApiClientService.GetApiDataAsync().ConfigureAwait(false);
        }
    }
}
