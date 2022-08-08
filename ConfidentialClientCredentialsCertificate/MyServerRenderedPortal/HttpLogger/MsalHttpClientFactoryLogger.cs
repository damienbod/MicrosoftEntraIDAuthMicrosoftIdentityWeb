using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Net.Http;

namespace ServiceApi.HttpLogger;

public class MsalHttpClientFactoryLogger : IMsalHttpClientFactory
{
    private static HttpClient _httpClient;

    public MsalHttpClientFactoryLogger(ILogger logger)
    {
        if (_httpClient == null)
        {
            _httpClient = new HttpClient(new MsalLoggingHandler(new HttpClientHandler(), logger));
        }
    }

    public HttpClient GetHttpClient()
    {
        return _httpClient;
    }
}