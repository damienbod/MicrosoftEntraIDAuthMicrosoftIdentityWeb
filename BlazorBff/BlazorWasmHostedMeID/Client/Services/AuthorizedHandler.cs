using System.Net;

namespace BlazorWasmHostedMeID.Client.Services;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
public class AuthorizedHandler : DelegatingHandler
{
    private readonly HostAuthenticationStateProvider _authenticationStateProvider;

    public AuthorizedHandler(HostAuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage;
        if (authState.User.Identity!= null && !authState.User.Identity.IsAuthenticated)
        {
            // if user is not authenticated, immediately set response status to 401 Unauthorized
            responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
        else
        {
            responseMessage = await base.SendAsync(request, cancellationToken);
        }

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();

            // if server returned 401 Unauthorized, redirect to login page
            if (content != null && content.Contains("acr")) // CAE
            {
                _authenticationStateProvider.CaeStepUp(content);
            }
            else // standard
            {
                _authenticationStateProvider.SignIn();
            }
        }

        return responseMessage;
    }
}
