using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
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
        private readonly ILogger<ApiService> _logger;

        public ApiService(IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ApiService>();
        }

        public async Task<JArray> GetApiDataAsync()
        {
            // Use Key Vault to get certificate
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            // Get the certificate from Key Vault
            var identifier = _configuration["CallApi:ClientCertificates:0:KeyVaultCertificateName"];
            var cert = await GetCertificateAsync(identifier).ConfigureAwait(false);

            var client = _clientFactory.CreateClient();

            var scope = _configuration["CallApi:ScopeForAccessToken"];
            var authority = $"{_configuration["CallApi:Instance"]}{_configuration["CallApi:TenantId"]}";

            // client credentials flows, get access token
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(_configuration["CallApi:ClientId"])
                    .WithAuthority(new Uri(authority))
                    .WithCertificate(cert)
                    .WithLogging(MyLoggingMethod, Microsoft.Identity.Client.LogLevel.Verbose,
                        enablePiiLogging: true, enableDefaultPlatformLogging: true)
                    .Build();

            var accessToken = await app.AcquireTokenForClient(new[] { scope }).ExecuteAsync().ConfigureAwait(false);

            client.BaseAddress = new Uri(_configuration["CallApi:ApiBaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // use access token and get payload
            var response = await client.GetAsync("weatherforecast").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var data = JArray.Parse(responseContent);

                return data;
            }

            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
        }

        private async Task<X509Certificate2> GetCertificateAsync(string identitifier)
        {
            var vaultBaseUrl = _configuration["CallApi:ClientCertificates:0:KeyVaultUrl"];
            var secretClient = new SecretClient(vaultUri: new Uri(vaultBaseUrl), credential: new DefaultAzureCredential());

            // Create a new secret using the secret client.
            var secretName = identitifier;
            //var secretVersion = "";
            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName).ConfigureAwait(false);

            var privateKeyBytes = Convert.FromBase64String(secret.Value);

            var certificateWithPrivateKey = new X509Certificate2(privateKeyBytes,
                (string)null,
                X509KeyStorageFlags.MachineKeySet);

            return certificateWithPrivateKey;
        }

        void MyLoggingMethod(Microsoft.Identity.Client.LogLevel level, string message, bool containsPii)
        {
            _logger.LogInformation($"MSAL {level} {containsPii} {message}");
        }
    }
}
