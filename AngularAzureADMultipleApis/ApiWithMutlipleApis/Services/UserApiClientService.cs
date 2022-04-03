using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiWithMutlipleApis.Services
{
    public class UserApiClientService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenAcquisition _tokenAcquisition;

        public UserApiClientService(
            ITokenAcquisition tokenAcquisition,
            IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<IEnumerable<string>> GetApiDataAsync()
        {

            var client = _clientFactory.CreateClient();

            var scopes = new List<string> { "api://b2a09168-54e2-4bc4-af92-a710a64ef1fa/access_as_user" };
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

            client.BaseAddress = new Uri("https://localhost:44395");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync("ApiForUserData");

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();

                var data = await JsonSerializer.DeserializeAsync<List<string>>(stream);

                if (data != null)
                    return data;

                return Array.Empty<string>();
            }

            throw new ApplicationException("oh no...");
        }
    }
}
