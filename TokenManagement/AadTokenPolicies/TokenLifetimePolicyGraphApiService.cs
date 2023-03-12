using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace TokenManagement;

public class TokenLifetimePolicyGraphApiService
{
    private readonly string graphUrl = "https://graph.microsoft.com/beta";

    private readonly string[] scopesPolicy = new string[] {
            "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" };

    private readonly string[] scopesApplications = new string[] {
            "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration", "Application.ReadWrite.All" };


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
        var graphclient = await GetGraphClient(scopesPolicy);

        return await graphclient
            .Policies
            .TokenLifetimePolicies
            .Request()
            .GetAsync();
    }

    public async Task<TokenLifetimePolicy> GetPolicy(string id)
    {
        var graphclient = await GetGraphClient(scopesPolicy);

        return await graphclient.Policies
            .TokenLifetimePolicies[id]
            .Request()
            .GetAsync();
    }

    public async Task<TokenLifetimePolicy> UpdatePolicy(TokenLifetimePolicy tokenLifetimePolicy)
    {
        var graphclient = await GetGraphClient(scopesPolicy);

        return await graphclient
            .Policies
            .TokenLifetimePolicies[tokenLifetimePolicy.Id]
            .Request()
            .UpdateAsync(tokenLifetimePolicy);
    }

    public async Task DeletePolicy(string policyId)
    {
        var graphclient = await GetGraphClient(scopesPolicy);

        await graphclient
            .Policies
            .TokenLifetimePolicies[policyId]
            .Request()
            .DeleteAsync();
    }

    public async Task<TokenLifetimePolicy> CreatePolicy(TokenLifetimePolicy tokenLifetimePolicy)
    {
        var graphclient = await GetGraphClient(scopesPolicy);

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
            .AddAsync(tokenLifetimePolicy);
    }

    public async Task<IStsPolicyAppliesToCollectionWithReferencesPage> PolicyAppliesTo(string tokenLifetimePolicyId)
    {
        var graphclient = await GetGraphClient(scopesPolicy);

        var appliesTo = await graphclient
            .Policies
            .TokenLifetimePolicies[tokenLifetimePolicyId]
            .AppliesTo
            .Request()
            .GetAsync();

        return appliesTo;
    }

    public async Task AssignTokenPolicyToApplicationUsingGraphId(string applicationGraphId, string tokenLifetimePolicyId)
    {
        var graphclient = await GetGraphClient(scopesApplications);

        var policy = await GetPolicy(tokenLifetimePolicyId);

        await graphclient
            .Applications[applicationGraphId]
            .TokenLifetimePolicies
            .References
            .Request()
            .AddAsync(policy);
    }

    public async Task<IGraphServiceApplicationsCollectionPage> GetApplicationsSingleOrMultipleOrg()
    {
        var graphclient = await GetGraphClient(scopesApplications);

        // AzureADMyOrg and AzureADMultipleOrgs
        return await graphclient
            .Applications
            .Request()
            .Expand("TokenLifetimePolicies")
            .Filter($"signInAudience eq 'AzureADMyOrg' or signInAudience eq 'AzureADMultipleOrgs'")
            .GetAsync();
    }

    public async Task<IGraphServiceApplicationsCollectionPage> GetApplications()
    {
        var graphclient = await GetGraphClient(scopesApplications);

        return await graphclient
            .Applications
            .Request()
            .Expand("TokenLifetimePolicies")
            .GetAsync();
    }

    public async Task AssignPolicyToApplication(string appId, TokenLifetimePolicy tokenLifetimePolicy)
    {
        var graphclient = await GetGraphClient(scopesApplications);

        var app2 = await graphclient
            .Applications
            .Request()
            .Filter($"appId eq '{appId}'")
            .GetAsync();

        var id = app2[0].Id;

        await graphclient
            .Applications[id]
            .TokenLifetimePolicies
            .References
            .Request()
            .AddAsync(tokenLifetimePolicy);
    }

    public async Task RemovePolicyFromApplication(string appId, string tokenLifetimePolicyId)
    {
        var graphclient = await GetGraphClient(scopesApplications);

        var app2 = await graphclient
            .Applications
            .Request()
            .Filter($"appId eq '{appId}'")
            .GetAsync();

        var id = app2[0].Id;

        await graphclient
            .Applications[id]
            .TokenLifetimePolicies[tokenLifetimePolicyId]
            .Reference
            .Request()
            .DeleteAsync();
    }

    private async Task<GraphServiceClient> GetGraphClient(string[] scopes)
    {
        var token = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri(graphUrl);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        GraphServiceClient graphClient = new GraphServiceClient(client)
        {
            AuthenticationProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            })
        };

        graphClient.BaseUrl = graphUrl;
        return graphClient;
    }
}