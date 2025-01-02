using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;

namespace WebAppWithRoles.Pages;

[AuthorizeForScopes(Scopes = new string[] { "api://5511b8d6-4652-4f2f-9643-59c61234e3c7/access_as_user", "user.read" })]
public class ClientWithRolesApiModel : PageModel
{
    private readonly ClientApiWithRolesService _apiService;

    public JArray? UserDataFromApi { get; set; }
    public JArray? StudentDataFromApi { get; set; }
    public JArray? AdminDataFromApi { get; set; }

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