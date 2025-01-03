using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace TokenManagement;

internal static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddTransient<TokenLifetimePolicyGraphApiService>();
        services.AddHttpClient();

        services.AddOptions();

        services.AddMicrosoftIdentityWebAppAuthentication(configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        services.AddRazorPages().AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }).AddMicrosoftIdentityUI();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        IdentityModelEventSource.ShowPII = true;
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        return app;
    }
}
