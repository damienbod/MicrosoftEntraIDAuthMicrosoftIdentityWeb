using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TokenManagement.Services;

namespace TokenManagement.Pages
{
    public class CallApiModel : PageModel
    {
        private TokenLifetimePolicyGraphApiService _tokenLifetimePolicyService;

        public CallApiModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyService)
        {
            _tokenLifetimePolicyService = tokenLifetimePolicyService;
        }

        public JArray DataFromApi { get; set; }

        public async Task OnGetAsync()
        {
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-configurable-token-lifetimes#configurable-token-lifetime-properties
            var tokenLifetimePolicy = new TokenLifetimePolicy
            {
                Definition = new List<string>()
                {
                    "{\"TokenLifetimePolicy\":{\"Version\":1,\"AccessTokenLifetime\":\"05:30:00\"}}"
                },
                DisplayName = "AppAccessTokenLifetimePolicy",
                IsOrganizationDefault = false
            };

            var data = await _tokenLifetimePolicyService.GetPolicies();
            //var created = await _tokenLifetimePolicyService.CreatePolicy(tokenLifetimePolicy);
            //await _tokenLifetimePolicyService.DeletePolicy(data[0].Id);
            await AssignTokenPolicyToApplication(data[0]);
        }

        private async Task AssignTokenPolicyToApplication(TokenLifetimePolicy tokenPolicy)
        {
            // You can only assign a policy to a single tenant application. ("signInAudience": "AzureADMyOrg")

            //var applicationId = "64ecb044-417b-4892-83d4-5c03e8c977b9"; // application id
            //var applicationId = "252278a5-c414-43ae-9363-34eed62463d0"; // single org
            var applicationId = "98328d53-55ec-4f14-8407-0ca5ff2f2d20"; // single org
            await _tokenLifetimePolicyService.AssignPolicyToApplication(applicationId, tokenPolicy);
        }
    }
}