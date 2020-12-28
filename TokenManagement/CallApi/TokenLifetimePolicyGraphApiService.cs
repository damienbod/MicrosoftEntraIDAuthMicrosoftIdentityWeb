using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TokenManagement.Services
{
    public class TokenLifetimePolicyGraphApiService
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IHttpClientFactory _clientFactory;

        public TokenLifetimePolicyGraphApiService(ITokenAcquisition tokenAcquisition,
            IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<IPolicyRootTokenLifetimePoliciesCollectionPage> GetPolicies()
        {
            var graphclient = await GetGraphClient(new string[] { 
                "Policy.ReadWrite.ApplicationConfiguration" })
               .ConfigureAwait(false);

            return await graphclient
                .Policies
                .TokenLifetimePolicies
                .Request()
                .GetAsync().ConfigureAwait(false);
        }

        public async Task<TokenLifetimePolicy> UpdatePolicy(TokenLifetimePolicy tokenLifetimePolicy)
        {
            var graphclient = await GetGraphClient(new string[] {
                "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" })
               .ConfigureAwait(false);

            return await graphclient.Policies.TokenLifetimePolicies["{id}"]
                .Request()
                .UpdateAsync(tokenLifetimePolicy);
        }

        public async Task DeletePolicy(string policyId)
        {
            var graphclient = await GetGraphClient(new string[] {
                "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" })
               .ConfigureAwait(false);

            await graphclient.Policies.TokenLifetimePolicies[policyId]
                .Request()
                .DeleteAsync();
        }

        public async Task<TokenLifetimePolicy> CreatePolicy(TokenLifetimePolicy tokenLifetimePolicy)
        {
            var graphclient = await GetGraphClient(new string[] { 
                "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" })
               .ConfigureAwait(false);

            //var tokenLifetimePolicy = new TokenLifetimePolicy
            //{
            //    Definition = new List<string>()
            //    {
            //        "{\"TokenLifetimePolicy\":{\"Version\":1,\"AccessTokenLifetime\":\"05:30:00\"}}"
            //    },
            //    DisplayName = "AppAccessTokenLifetimePolicy",
            //    IsOrganizationDefault = false
            //};

            return await graphclient
                .Policies
                .TokenLifetimePolicies
                .Request()
                .AddAsync(tokenLifetimePolicy)
                .ConfigureAwait(false);
        }

        public async Task AssignPolicyToApplication(string applicationId, 
            TokenLifetimePolicy tokenLifetimePolicy)
        {
            var graphclient = await GetGraphClient(new string[] {
                "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration", "Application.ReadWrite.All" })
              .ConfigureAwait(false);

            var app2 = await graphclient
                .Applications
                .Request().Filter($"appId eq '{applicationId}'")
                .GetAsync()
                .ConfigureAwait(false);

            var id = app2[0].Id;

            var tokenIssuancePolicies = await graphclient
                .Applications[id]
                .TokenIssuancePolicies
                .Request()
                .GetAsync()
                .ConfigureAwait(false);

            await graphclient
                .Applications[id]
                .TokenLifetimePolicies[tokenLifetimePolicy.Id]
                .Reference
                .Request()
                .DeleteAsync();

            await graphclient
                .Applications[id]
                .TokenLifetimePolicies
                .References
                .Request()
                .AddAsync(tokenLifetimePolicy)
                .ConfigureAwait(false);
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
