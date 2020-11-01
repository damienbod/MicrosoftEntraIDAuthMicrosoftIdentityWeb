using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WebAppUserApis.Pages
{
    public class CallApiModel : PageModel
    {
        private readonly UserApiOneService _apiService;

        public JArray DataFromApi { get; set; }
        public CallApiModel(UserApiOneService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _apiService.GetApiDataAsync();
        }
    }
}