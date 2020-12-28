using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TokenManagement.Services;

namespace TokenManagement.Pages
{
    public class CallApiModel : PageModel
    {
        private GraphApiClientService _graphApiClientService;

        public CallApiModel(GraphApiClientService graphApiClientService)
        {
            _graphApiClientService = graphApiClientService;
        }

        public JArray DataFromApi { get; set; }

        public async Task OnGetAsync()
        {
            var data = await _graphApiClientService.GetPolicies();
        }
    }
}