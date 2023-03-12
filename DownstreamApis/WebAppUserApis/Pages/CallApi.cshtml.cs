using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;

namespace WebAppUserApis.Pages;

[AuthorizeForScopes(Scopes = new string[] { "api://b2a09168-54e2-4bc4-af92-a710a64ef1fa/access_as_user" })]
public class CallApiModel : PageModel
{
    private readonly UserApiOneService _apiService;

    public JArray? DataFromApi { get; set; }

    public CallApiModel(UserApiOneService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        DataFromApi = await _apiService.GetApiDataAsync();
    }
}