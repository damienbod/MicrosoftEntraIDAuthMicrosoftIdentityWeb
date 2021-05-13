using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace MyServerRenderedPortal.Pages
{
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