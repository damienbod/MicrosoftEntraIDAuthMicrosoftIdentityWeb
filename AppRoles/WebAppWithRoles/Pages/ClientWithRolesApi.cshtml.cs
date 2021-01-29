using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WebAppWithRoles.Pages
{
    public class ClientWithRolesApiModel : PageModel
    {
        private readonly ClientApiWithRolesService _apiService;

        public JArray DataFromApi { get; set; }
        public ClientWithRolesApiModel(ClientApiWithRolesService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _apiService.GetUserDataFromApi();
        }
    }
}