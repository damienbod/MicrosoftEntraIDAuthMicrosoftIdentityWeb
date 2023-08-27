using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client;
using ServiceApi.HttpLogger;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace MyServerRenderedPortal;

public class ConfidentialClientApiService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfidentialClientApiService> _logger;

    public ConfidentialClientApiService(IHttpClientFactory clientFactory,
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger<ConfidentialClientApiService>();
    }

    public async Task<IEnumerable<WeatherForecast>?> GetApiDataAsync()
    {
        // Use Key Vault to get certificate

        // Get the certificate from Key Vault
        var identifier = _configuration["CallApi:ClientCertificates:0:KeyVaultCertificateName"];
        var cert = await GetCertificateAsync(identifier);

        var client = _clientFactory.CreateClient();

        var scope = _configuration["CallApi:ScopeForAccessToken"];
        var authority = $"{_configuration["CallApi:Instance"]}{_configuration["CallApi:TenantId"]}";

        // client credentials flows, get access token
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(_configuration["CallApi:ClientId"])
                .WithAuthority(new Uri(authority))
                .WithHttpClientFactory(new MsalHttpClientFactoryLogger(_logger))
                .WithCertificate(cert)
                .WithLogging(MyLoggingMethod, Microsoft.Identity.Client.LogLevel.Verbose,
                    enablePiiLogging: true, enableDefaultPlatformLogging: true)
                .Build();

        var accessToken = await app.AcquireTokenForClient(new[] { scope }).ExecuteAsync();

        client.BaseAddress = new Uri(_configuration["CallApi:ApiBaseAddress"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // use access token and get payload
        var response = await client.GetAsync("weatherforecast");
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(responseContent);

            return data;
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }

    private async Task<X509Certificate2> GetCertificateAsync(string? identitifier)
    {
        var vaultBaseUrl = _configuration["CallApi:ClientCertificates:0:KeyVaultUrl"];
        vaultBaseUrl ??= "https://damienbod.vault.azure.net";

        var tenantId = _configuration["CallApi:TenantId"];
        var clientId = _configuration["CallApi:ClientId"];
        var clientSecretKeyVaultAccess = _configuration["ClientSecretKeyVaultAccess"];

        var secretClient = new SecretClient(vaultUri: new Uri(vaultBaseUrl),
            credential: new ClientSecretCredential(tenantId, clientId, clientSecretKeyVaultAccess));

        // Create a new secret using the secret client.
        var secretName = identitifier;
        //var secretVersion = "";
        KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);

        var privateKeyBytes = Convert.FromBase64String(secret.Value);

        var certificateWithPrivateKey = new X509Certificate2(privateKeyBytes,
            string.Empty, X509KeyStorageFlags.MachineKeySet);

        return certificateWithPrivateKey;
    }

    void MyLoggingMethod(Microsoft.Identity.Client.LogLevel level, string message, bool containsPii)
    {
        _logger.LogInformation("MSAL {level} {containsPii} {message}", level, containsPii, message);
    }
}