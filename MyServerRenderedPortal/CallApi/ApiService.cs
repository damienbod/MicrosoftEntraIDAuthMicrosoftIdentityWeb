using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MyServerRenderedPortal
{
    public class ApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public ApiService(IHttpClientFactory clientFactory, 
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<JArray> GetApiDataAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient();

                var scope = _configuration["CallApi:ScopeForAccessToken"];
                var authority = $"{_configuration["CallApi:Instance"]}{_configuration["CallApi:TenantId"]}";
                var cert = new X509Certificate2("damienbod-ServiceApiCert-20200926.pfx");
                IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(_configuration["CallApi:ClientId"])
                        .WithAuthority(new Uri(authority))
                        .WithCertificate(cert)
                        .Build();

                var accessToken = await app.AcquireTokenForClient(new[] { scope }).ExecuteAsync();

                client.BaseAddress = new Uri(_configuration["CallApi:ApiBaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
       
                var response = await client.GetAsync("weatherforecast");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
