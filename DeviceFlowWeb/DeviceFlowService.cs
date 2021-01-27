using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceFlowWeb
{
    public class DeviceFlowService
    {
        private readonly AzureAdConfiguration _azureAdConfiguration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly DiscoveryDocumentRequest _discoveryDocumentRequest;

        public DeviceFlowService(IOptions<AzureAdConfiguration> azureAdConfiguration, IHttpClientFactory clientFactory)
        {
            _azureAdConfiguration = azureAdConfiguration.Value;
            _clientFactory = clientFactory;
            var idpEndpoint = $"{_azureAdConfiguration.Instance}{_azureAdConfiguration.TenantId}/v2.0";
            _discoveryDocumentRequest = new DiscoveryDocumentRequest
            {
                Address = idpEndpoint,
                Policy = new DiscoveryPolicy
                {
                    // turned off => Azure AD uses different domains.
                    ValidateEndpoints = false
                }
            };
        }

        public async Task<DeviceAuthorizationResponse> GetDeviceCode()
        {
            var client = _clientFactory.CreateClient();

            var disco = await HttpClientDiscoveryExtensions
                .GetDiscoveryDocumentAsync(client, _discoveryDocumentRequest);

            if (disco.IsError)
            {
                throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
            }

            var deviceAuthorizationRequest = new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = _azureAdConfiguration.ClientId
            };
            deviceAuthorizationRequest.Scope = "email profile openid";
            var response = await client.RequestDeviceAuthorizationAsync(deviceAuthorizationRequest);

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            return response;
        }

        public async Task<TokenResponse> PollTokenRequests(string deviceCode, int interval)
        {
            var client = _clientFactory.CreateClient();

            var disco = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(client, _discoveryDocumentRequest);

            if (disco.IsError)
            {
                throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
            }

            while (true)
            {
                if(!string.IsNullOrWhiteSpace(deviceCode))
                {
                    var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = _azureAdConfiguration.ClientId,
                        DeviceCode = deviceCode
                    });

                    if (response.IsError)
                    {
                        if (response.Error == "authorization_pending" || response.Error == "slow_down")
                        {
                            Console.WriteLine($"{response.Error}...waiting.");
                            await Task.Delay(interval * 1000);
                        }
                        else
                        {
                            throw new Exception(response.Error);
                        }
                    }
                    else
                    {
                        return response;
                    }
                }
                else
                {
                    await Task.Delay(interval * 1000);
                }
            }
        }

    }
}
