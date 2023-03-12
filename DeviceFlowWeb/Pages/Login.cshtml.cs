using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeviceFlowWeb.Pages;

public class LoginModel : PageModel
{
    private readonly DeviceFlowService _deviceFlowService;
    private readonly AuthenticationSignInService _authenticationSignInService;

    public string? AuthenticatorUri { get; set; }

    public string? UserCode { get; set; }

    public LoginModel(DeviceFlowService deviceFlowService, AuthenticationSignInService authenticationSignInService)
    {
        _deviceFlowService = deviceFlowService;
        _authenticationSignInService = authenticationSignInService;
    }

    public async Task OnGetAsync()
    {
        HttpContext.Session.SetString("DeviceCode", string.Empty);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var deviceAuthorizationResponse = await _deviceFlowService.GetDeviceCode();
        AuthenticatorUri = deviceAuthorizationResponse.VerificationUri;
        UserCode = deviceAuthorizationResponse.UserCode;

        if (string.IsNullOrEmpty(HttpContext.Session.GetString("DeviceCode")))
        {
            HttpContext.Session.SetString("DeviceCode", deviceAuthorizationResponse.DeviceCode);
            HttpContext.Session.SetInt32("Interval", deviceAuthorizationResponse.Interval);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var deviceCode = HttpContext.Session.GetString("DeviceCode");
        var interval = HttpContext.Session.GetInt32("Interval");

        if(interval.GetValueOrDefault() <= 0)
        {
            interval = 5;
        }

        if(deviceCode != null && interval != null)
        {
            var tokenresponse = await _deviceFlowService.PollTokenRequests(deviceCode, interval.Value);

            if (tokenresponse.IsError)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            await _authenticationSignInService.SignIn(HttpContext,
            tokenresponse.AccessToken,
            tokenresponse.IdentityToken,
            tokenresponse.ExpiresIn);
        }

        return Redirect("/Index");
    }
}