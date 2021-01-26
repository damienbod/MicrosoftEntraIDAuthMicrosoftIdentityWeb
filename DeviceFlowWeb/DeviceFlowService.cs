using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceFlowWeb
{
    public class DeviceFlowService
    {
        private readonly IOptions<AuthConfigurations> _authConfigurations;
        private readonly IHttpClientFactory _clientFactory;
        private readonly DiscoveryDocumentRequest _discoveryDocumentRequest;

        public DeviceFlowService(IOptions<AuthConfigurations> authConfigurations, IHttpClientFactory clientFactory)
        {
            _authConfigurations = authConfigurations;
            _clientFactory = clientFactory;
            _discoveryDocumentRequest = new DiscoveryDocumentRequest
            {
                Address = _authConfigurations.Value.StsServer,
                Policy = new DiscoveryPolicy
                {
                    ValidateEndpoints = false
                }
            };
        }

        internal async Task<DeviceAuthorizationResponse> BeginLogin()
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
                ClientId = "f81baf3d-f8f3-4976-8b5a-798ff57daab5"
            };
            deviceAuthorizationRequest.Scope = "email profile openid";
            var response = await client.RequestDeviceAuthorizationAsync(deviceAuthorizationRequest);

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            return response;
        }

        internal async Task<TokenResponse> RequestTokenAsync(string deviceCode, int interval)
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
                        ClientId = "f81baf3d-f8f3-4976-8b5a-798ff57daab5",
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
