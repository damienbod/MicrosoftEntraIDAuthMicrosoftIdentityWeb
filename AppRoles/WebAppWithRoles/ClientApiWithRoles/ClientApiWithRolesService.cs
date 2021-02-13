using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebAppWithRoles
{
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

        public async Task<JArray> GetUserDataFromApi()
        {
            return await GetDataFromApi("userdata");
        }

        public async Task<JArray> GetStudentDataFromApi()
        {
            return await GetDataFromApi("studentdata");
        }

        public async Task<JArray> GetAdminDataFromApi()
        {
            return await GetDataFromApi("admindata");
        }


        private async Task<JArray> GetDataFromApi(string path)
        {
            var client = _clientFactory.CreateClient();

            var scope = _configuration["ApiWithRoles:ScopeForAccessToken"];
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

            client.BaseAddress = new Uri(_configuration["ApiWithRoles:ApiBaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync($"api/{path}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(responseContent);

                return data;
            }
            var errorList = new List<string> { $"Status code: {response.StatusCode}", $"Error: {response.ReasonPhrase}" };
            return JArray.FromObject(errorList);
        }
    }
}
