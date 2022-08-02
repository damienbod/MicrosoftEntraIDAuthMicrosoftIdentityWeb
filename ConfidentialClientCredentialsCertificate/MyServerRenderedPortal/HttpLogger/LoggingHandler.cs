using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceApi.HttpLogger;

public class MsalLoggingHandler : DelegatingHandler
{
    private ILogger _logger;

    public MsalLoggingHandler(HttpMessageHandler innerHandler, ILogger logger)
        : base(innerHandler)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("MSAL Request: {request}", request.ToString());
        if (request.Content != null)
        {
            _logger.LogDebug(await request.Content.ReadAsStringAsync());
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        _logger.LogDebug("MSAL Response: {response}", response.ToString());
        if (response.Content != null)
        {
            _logger.LogDebug(await response.Content.ReadAsStringAsync());
        }

        return response;
    }
}
