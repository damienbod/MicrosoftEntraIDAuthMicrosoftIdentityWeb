﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace UserApiOne.Controllers
{
    [Authorize]
    [AuthorizeForScopes(Scopes = new string[] { "api://72286b8d-5010-4632-9cea-e69e565a5517/user_impersonation" })]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly UserApiTwoService _apiService;

        public List<WeatherForecast>? DataFromApi { get; set; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController(UserApiTwoService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>?> Get()
        {
            var scopeRequiredByApi = new string[] { "access_as_user" };
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            DataFromApi = await _apiService.GetApiDataAsync();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
