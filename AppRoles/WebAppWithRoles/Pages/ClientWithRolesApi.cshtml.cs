using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WebAppWithRoles.Pages
{
    public class ClientWithRolesApiModel : PageModel
    {
        private readonly ClientApiWithRolesService _apiService;

        public JArray UserDataFromApi { get; set; }
        public JArray StudentDataFromApi { get; set; }
        public JArray AdminDataFromApi { get; set; }
        public ClientWithRolesApiModel(ClientApiWithRolesService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            UserDataFromApi = await _apiService.GetUserDataFromApi();
            StudentDataFromApi = await _apiService.GetStudentDataFromApi();
            AdminDataFromApi = await _apiService.GetAdminDataFromApi();
        }
    }
}