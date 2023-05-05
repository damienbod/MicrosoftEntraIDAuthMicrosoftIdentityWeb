using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.IdentityModel.Tokens.Jwt;

namespace WebAppWithRoles;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<ClientApiWithRolesService>();
        services.AddHttpClient();

        services.AddOptions();

        services.AddDistributedMemoryCache();
        string[]? initialScopes = Configuration.GetValue<string>(
            "ApiWithRoles:ScopeForAccessToken")?.Split(' ');

        //services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
        //    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
        //    .AddDistributedTokenCaches();

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
                {
                    Configuration.Bind("AzureAd", options);
                    options.UsePkce = true;
                }, 
                options => { Configuration.Bind("AzureAd", options); }
            )
            .EnableTokenAcquisitionToCallDownstreamApi(
                options => Configuration.Bind("AzureAd", options), initialScopes)
            .AddDistributedTokenCaches();

        

        services.AddRazorPages().AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }).AddMicrosoftIdentityUI();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            
        if (env.IsDevelopment())
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}