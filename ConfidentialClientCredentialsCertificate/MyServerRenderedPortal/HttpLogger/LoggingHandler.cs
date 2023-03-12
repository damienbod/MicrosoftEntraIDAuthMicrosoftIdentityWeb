using System.Text;

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
        var builder = new StringBuilder();
        builder.AppendLine("MSAL Request: {request}");
        builder.AppendLine(request.ToString());
        if (request.Content != null)
        {
            builder.AppendLine();
            builder.AppendLine(await request.Content.ReadAsStringAsync());
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        builder.AppendLine();
        builder.AppendLine("MSAL Response: {response}");
        builder.AppendLine(response.ToString());
        if (response.Content != null)
        {
            builder.AppendLine();
            builder.AppendLine(await response.Content.ReadAsStringAsync());
        }

        _logger.LogDebug(builder.ToString());

        return response;
    }
}
