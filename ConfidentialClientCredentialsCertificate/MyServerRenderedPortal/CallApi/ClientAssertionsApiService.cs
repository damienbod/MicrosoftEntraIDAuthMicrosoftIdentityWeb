using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ServiceApi.HttpLogger;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace MyServerRenderedPortal;

public class ClientAssertionsApiService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfidentialClientApiService> _logger;

    public ClientAssertionsApiService(IHttpClientFactory clientFactory,
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

        if (cert == null)
            throw new Exception("cert cannot be null");

        var client = _clientFactory.CreateClient();

        var scope = _configuration["CallApi:ScopeForAccessToken"];
        var authority = $"{_configuration["CallApi:Instance"]}{_configuration["CallApi:TenantId"]}";

        string signedClientAssertion = GetSignedClientAssertion(cert, 
            _configuration["CallApi:TenantId"], _configuration["CallApi:ClientId"]);

        var app = ConfidentialClientApplicationBuilder
                .Create(_configuration["CallApi:ClientId"])
                .WithAuthority(new Uri(authority))
                .WithHttpClientFactory(new MsalHttpClientFactoryLogger(_logger))
                .WithClientAssertion(signedClientAssertion)
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

        var secretClient = new SecretClient(vaultUri: new Uri(vaultBaseUrl), credential: new DefaultAzureCredential());

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

    static string GetSignedClientAssertion(X509Certificate2 certificate, string tenantId, string confidentialClientID)
    {
        //aud = https://login.microsoftonline.com/ + Tenant ID + /v2.0
        string aud = $"https://login.microsoftonline.com/{tenantId}/v2.0";

        // no need to add exp, nbf as JsonWebTokenHandler will add them by default.
        var claims = new Dictionary<string, object>()
            {
                { "aud", aud },
                { "iss", confidentialClientID },
                { "jti", Guid.NewGuid().ToString() },
                { "sub", confidentialClientID }
            };

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            SigningCredentials = new X509SigningCredentials(certificate)
        };

        var handler = new JsonWebTokenHandler();
        var signedClientAssertion = handler.CreateToken(securityTokenDescriptor);

        return signedClientAssertion;
    }
}