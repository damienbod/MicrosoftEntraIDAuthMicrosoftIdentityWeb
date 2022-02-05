using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Threading.Tasks;

namespace BlazorAzureADWithApis.Server.Services.Application
{
    public class MicrosoftGraphApplicationClient
    {
        private readonly IConfiguration _configuration;

        public MicrosoftGraphApplicationClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IUserAppRoleAssignmentsCollectionPage> GetGraphUserAppRoles(string objectIdentifier)
        {
            var graphServiceClient = GetGraphClient();

            return await graphServiceClient.Users[objectIdentifier]
                    .AppRoleAssignments
                    .Request()
                    .GetAsync();
        }

        public async Task<IDirectoryObjectGetMemberGroupsCollectionPage> GetGraphUserMemberGroups(string objectIdentifier)
        {
            var securityEnabledOnly = true;

            var graphServiceClient = GetGraphClient();

            return await graphServiceClient.Users[objectIdentifier]
                .GetMemberGroups(securityEnabledOnly)
                .Request().PostAsync();
        }

        private async Task<string> GetUserIdAsync(string email, GraphServiceClient graphServiceClient)
        {
            var filter = $"userPrincipalName eq '{email}'";
            //var filter = $"startswith(userPrincipalName,'{email}')";

            var users = await graphServiceClient.Users
                .Request()
                .Filter(filter)
                .GetAsync();

            if (users.CurrentPage.Count == 0)
            {
                return string.Empty;
            }
            return users.CurrentPage[0].Id;
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
}
