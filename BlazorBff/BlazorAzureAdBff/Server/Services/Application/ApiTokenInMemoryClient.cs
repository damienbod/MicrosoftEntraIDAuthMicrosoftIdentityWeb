using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlazorAzureADWithApis.Server.Services.Application
{
    public class ApiTokenInMemoryClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ApiTokenInMemoryClient> _logger;

        private readonly IConfiguration _configuration;
        private readonly IConfidentialClientApplication _app;
        private readonly ConcurrentDictionary<string, AccessTokenItem> _accessTokens = new();

        private class AccessTokenItem
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresIn { get; set; }
        }

        public ApiTokenInMemoryClient(IHttpClientFactory clientFactory,
            IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ApiTokenInMemoryClient>();
            _app = InitConfidentialClientApplication();
        }

        public async Task<GraphServiceClient> GetGraphClient()
        {
            var result = await GetApiToken("default");

            var httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var graphClient = new GraphServiceClient(httpClient)
            {
                AuthenticationProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result);
                    await Task.FromResult<object>(null);
                })
            };

            return graphClient;
        }

        private async Task<string> GetApiToken(string api_name)
        {
            if (_accessTokens.ContainsKey(api_name))
            {
                var accessToken = _accessTokens.GetValueOrDefault(api_name);
                if (accessToken.ExpiresIn > DateTime.UtcNow)
                {
                    return accessToken.AccessToken;
                }
                else
                {
                    // remove
                    _accessTokens.TryRemove(api_name, out _);
                }
            }

            _logger.LogDebug($"GetApiToken new from STS for {api_name}");

            // add
            var newAccessToken = await AcquireTokenSilent();
            _accessTokens.TryAdd(api_name, newAccessToken);

            return newAccessToken.AccessToken;
        }

        private async Task<AccessTokenItem> AcquireTokenSilent()
        {
            //var scopes = "User.read Mail.Send Mail.ReadWrite OnlineMeetings.ReadWrite.All";
            var authResult = await _app
                .AcquireTokenForClient(scopes: new[] { "https://graph.microsoft.com/.default" })
                .WithAuthority(AzureCloudInstance.AzurePublic, _configuration["AzureAd:TenantId"])
                .ExecuteAsync();

            return new AccessTokenItem
            {
                ExpiresIn = authResult.ExpiresOn.UtcDateTime,
                AccessToken = authResult.AccessToken
            };
        }

        private IConfidentialClientApplication InitConfidentialClientApplication()
        {
            return ConfidentialClientApplicationBuilder
                .Create(_configuration["AzureAd:ClientId"])
                .WithClientSecret(_configuration["AzureAd:ClientSecret"])
                .Build();
        }
    }
}
