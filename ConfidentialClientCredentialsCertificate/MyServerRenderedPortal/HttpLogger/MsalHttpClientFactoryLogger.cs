using Microsoft.Identity.Client;

namespace ServiceApi.HttpLogger;

public class MsalHttpClientFactoryLogger : IMsalHttpClientFactory
{
    private static HttpClient? _httpClient;
    private readonly ILogger _logger;

    public MsalHttpClientFactoryLogger(ILogger logger)
    {
        _logger = logger;
    }

    public HttpClient GetHttpClient()
    {
        _httpClient ??= new HttpClient(new MsalLoggingHandler(new HttpClientHandler(), _logger));

        return _httpClient;
    }
}