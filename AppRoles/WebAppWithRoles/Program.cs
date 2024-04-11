using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.JsonWebTokens;
using WebAppWithRoles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ClientApiWithRolesService>();
builder.Services.AddHttpClient();

builder.Services.AddOptions();

builder.Services.AddDistributedMemoryCache();
string[]? initialScopes = builder.Configuration.GetValue<string>(
    "ApiWithRoles:ScopeForAccessToken")?.Split(' ');

//builder.Services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
//    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
//    .AddDistributedTokenCaches();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.UsePkce = true;
    },
        options => { builder.Configuration.Bind("AzureAd", options); }
    )
    .EnableTokenAcquisitionToCallDownstreamApi(
        options => builder.Configuration.Bind("AzureAd", options), initialScopes)
    .AddDistributedTokenCaches();

builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

var app = builder.Build();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
// IdentityModelEventSource.ShowPII = true;

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
