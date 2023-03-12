using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyServerRenderedPortal.Pages;

public class CallApiModel : PageModel
{
    
    private readonly ClientAssertionsApiService _clientAssertionsApiService;
    private readonly ConfidentialClientApiService _confidentialClientApiService;

    public IEnumerable<WeatherForecast>? DataFromApi { get; set; }

    public CallApiModel(ConfidentialClientApiService apiService,
        ClientAssertionsApiService clientAssertionsApiService)
    {
        _confidentialClientApiService = apiService;
        _clientAssertionsApiService = clientAssertionsApiService;
    }

    public async Task OnGetAsync()
    {
        DataFromApi = await _confidentialClientApiService.GetApiDataAsync();
        //DataFromApi = await _clientAssertionsApiService.GetApiDataAsync();
    }
}