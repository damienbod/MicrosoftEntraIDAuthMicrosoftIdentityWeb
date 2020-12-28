using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TokenManagement.Services;

namespace TokenManagement.Pages
{
    public class CallApiModel : PageModel
    {
        private TokenLifetimePolicyGraphApiService _graphApiClientService;

        public CallApiModel(TokenLifetimePolicyGraphApiService graphApiClientService)
        {
            _graphApiClientService = graphApiClientService;
        }

        public JArray DataFromApi { get; set; }

        public async Task OnGetAsync()
        {
            var data = await _graphApiClientService.GetPolicies();
            //var created = await _graphApiClientService.CreatePolicy();

            await AssignTokenPolicyToApplication(data[1]);
        }

        private async Task AssignTokenPolicyToApplication(TokenLifetimePolicy tokenPolicy)
        {
            // You can only assign a policy to a single tenant application. ("signInAudience": "AzureADMyOrg")

            var applicationId = "64ecb044-417b-4892-83d4-5c03e8c977b9"; // application id
            //var applicationId = "252278a5-c414-43ae-9363-34eed62463d0"; // single org
            await _graphApiClientService.AssignPolicyToApplication(applicationId, tokenPolicy);
        }
    }
}