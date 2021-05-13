using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace PortalDecryptionCertificates.Pages
{
    [AuthorizeForScopes(Scopes = new string[] { "api://fdc48df2-2b54-411b-a684-7d9868ce1a95/access_as_user" })]
    public class CallApiModel : PageModel
    {
        private readonly ApiService _apiService;

        public JArray DataFromApi { get; set; }
        public CallApiModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _apiService.GetApiDataAsync().ConfigureAwait(false);
        }
    }
}