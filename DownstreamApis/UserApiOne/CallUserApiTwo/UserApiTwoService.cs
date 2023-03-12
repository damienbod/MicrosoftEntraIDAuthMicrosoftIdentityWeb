using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using System.Text.Json;

namespace UserApiOne;

public class UserApiTwoService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public UserApiTwoService(IHttpClientFactory clientFactory, 
        ITokenAcquisition tokenAcquisition, 
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<List<WeatherForecast>?> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        // user_impersonation access_as_user access_as_application .default
        var scope = _configuration["UserApiTwo:ScopeForAccessToken"];
        if(scope == null) throw new ArgumentNullException(nameof(scope));
        
        var uri = _configuration["UserApiTwo:ApiBaseAddress"];
        if (uri == null) throw new ArgumentNullException(nameof(uri));

        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

        client.BaseAddress = new Uri(uri);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("weatherforecast");
        if (response.IsSuccessStatusCode)
        {
            var data = await JsonSerializer.DeserializeAsync<List<WeatherForecast>?>(
                await response.Content.ReadAsStreamAsync());

            return data;
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }
}