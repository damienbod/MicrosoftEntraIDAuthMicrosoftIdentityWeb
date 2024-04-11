using Azure.Identity;
using Microsoft.Graph.Users.Item.GetMemberGroups;
using Microsoft.Graph.Models;
using Microsoft.Graph;

namespace BlazorWasmHostedMeID.Server.Services.Application;

public class MsGraphApplicationService
{
    private readonly IConfiguration _configuration;

    public MsGraphApplicationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AppRoleAssignmentCollectionResponse?> GetGraphApiUserAppRoles(string userId)
    {
        var graphServiceClient = GetGraphClient();

        return await graphServiceClient.Users[userId]
            .AppRoleAssignments
            .GetAsync();
    }

    public async Task<GetMemberGroupsPostResponse?> GetGraphApiUserMemberGroups(string userId)
    {
        var graphServiceClient = GetGraphClient();

        var requestBody = new GetMemberGroupsPostRequestBody
        {
            SecurityEnabledOnly = true,
        };

        return await graphServiceClient.Users[userId]
            .GetMemberGroups
            .PostAsGetMemberGroupsPostResponseAsync(requestBody);
    }
    private GraphServiceClient GetGraphClient()
    {
        string[] scopes = new[] { "https://graph.microsoft.com/.default" };
        var tenantId = _configuration["AzureAd:TenantId"];

        // Values from app registration
        var clientId = _configuration.GetValue<string>("AzureAd:ClientId");
        var clientSecret = _configuration.GetValue<string>("AzureAd:ClientSecret");

        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);

        return new GraphServiceClient(clientSecretCredential, scopes);
    }

}
