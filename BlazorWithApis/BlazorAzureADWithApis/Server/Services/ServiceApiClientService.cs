using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorAzureADWithApis.Server.Services;

public class ServiceApiClientService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;

    public ServiceApiClientService(
        ITokenAcquisition tokenAcquisition,
        IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
    }

    public async Task<IEnumerable<string>?> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        // CC flow access_as_application" (App Role in Azure AD app registration)
        var scope = "api://b178f3a5-7588-492a-924f-72d7887b7e48/.default"; 
        var accessToken = await _tokenAcquisition.GetAccessTokenForAppAsync(scope);

        client.BaseAddress = new Uri("https://localhost:44324");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("ApiForServiceData");

        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();

            var payload = await JsonSerializer.DeserializeAsync<List<string>>(stream);

            return payload;
        }

        throw new ApplicationException("oh no...");
    }
}