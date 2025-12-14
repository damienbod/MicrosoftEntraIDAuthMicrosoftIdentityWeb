using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace WebAppWithRoles;

public class ClientApiWithRolesService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public ClientApiWithRolesService(IHttpClientFactory clientFactory,
        ITokenAcquisition tokenAcquisition,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<string> GetUserDataFromApi()
    {
        return await GetDataFromApi("userdata");
    }

    public async Task<string> GetStudentDataFromApi()
    {
        return await GetDataFromApi("studentdata");
    }

    public async Task<string> GetAdminDataFromApi()
    {
        return await GetDataFromApi("admindata");
    }


    private async Task<string> GetDataFromApi(string path)
    {
        var client = _clientFactory.CreateClient();

        var scope = _configuration["ApiWithRoles:ScopeForAccessToken"];
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync([scope!]);

        client.BaseAddress = new Uri(_configuration["ApiWithRoles:ApiBaseAddress"]!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync($"api/{path}");
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
        var errorList = $"Status code: {response.StatusCode} Error: {response.ReasonPhrase}";

        return errorList;
    }
}