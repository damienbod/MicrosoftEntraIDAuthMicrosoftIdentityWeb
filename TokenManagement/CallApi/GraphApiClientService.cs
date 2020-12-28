using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TokenManagement.Services
{
    public class GraphApiClientService
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IHttpClientFactory _clientFactory;

        public GraphApiClientService(ITokenAcquisition tokenAcquisition,
            IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<IPolicyRootTokenLifetimePoliciesCollectionPage> GetPolicies()
        {
            var graphclient = await GetGraphClient(new string[] { "Policy.ReadWrite.ApplicationConfiguration" })
               .ConfigureAwait(false);

            return await graphclient
                .Policies
                .TokenLifetimePolicies
                .Request()
                .GetAsync().ConfigureAwait(false);
        }

        public async Task<TokenLifetimePolicy> CreatePolicy()
        {
            var graphclient = await GetGraphClient(new string[] { "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" })
               .ConfigureAwait(false);

            var tokenLifetimePolicy = new TokenLifetimePolicy
            {
                Definition = new List<string>()
                {
                    "{\"TokenLifetimePolicy\":{\"Version\":1,\"AccessTokenLifetime\":\"01:30:00\"}}"
                },
                DisplayName = "MyAccessTokenLifetimePolicy",
                IsOrganizationDefault = true
            };

            return await graphclient.Policies.TokenLifetimePolicies
                .Request()
                .AddAsync(tokenLifetimePolicy).ConfigureAwait(false);
        }

        private async Task<GraphServiceClient> GetGraphClient(string[] scopes)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(
             scopes).ConfigureAwait(false);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://graph.microsoft.com/beta");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GraphServiceClient graphClient = new GraphServiceClient(client)
            {
                AuthenticationProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                })
            };

            graphClient.BaseUrl = "https://graph.microsoft.com/beta";
            return graphClient;
        }
    }
}
