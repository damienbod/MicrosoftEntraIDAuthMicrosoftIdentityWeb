using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyServerRenderedPortal.Pages;

public class CallApiModel : PageModel
{
    private readonly ConfidentialClientApiService _apiService;

    public IEnumerable<WeatherForecast>? DataFromApi { get; set; }

    public CallApiModel(ConfidentialClientApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        DataFromApi = await _apiService.GetApiDataAsync();
    }
}