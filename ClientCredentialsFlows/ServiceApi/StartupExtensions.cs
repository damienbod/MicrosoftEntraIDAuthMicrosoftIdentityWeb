using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using Serilog;

namespace ServiceApi;

internal static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddSecurityHeaderPolicies()
          .SetPolicySelector((PolicySelectorContext ctx) =>
          {
              return SecurityHeadersDefinitions.GetHeaderPolicyCollection(
                  builder.Environment.IsDevelopment());
          });

        services.AddSingleton<IAuthorizationHandler, HasServiceApiRoleHandler>();

        services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

        services.AddControllers();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
            {
                validateAccessTokenPolicy.Requirements.Add(new HasServiceApiRoleRequirement());

                // Validate id of application for which the token was created
                // In this case the CC client application 
                validateAccessTokenPolicy.RequireClaim("azp", "2b50a014-f353-4c10-aace-024f19a55569");

                // only allow tokens which used "Private key JWT Client authentication"
                // // https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens
                // Indicates how the client was authenticated. For a public client, the value is "0". 
                // If client ID and client secret are used, the value is "1". 
                // If a client certificate was used for authentication, the value is "2".
                validateAccessTokenPolicy.RequireClaim("azpacr", "1");
            });
        });
        services.AddOpenApi(options =>
        {
            //options.UseTransformer((document, context, cancellationToken) =>
            //{
            //    document.Info = new()
            //    {
            //        Title = "My API",
            //        Version = "v1",
            //        Description = "API for Damien"
            //    };
            //    return Task.CompletedTask;
            //});
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        IdentityModelEventSource.ShowPII = true;
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        app.UseSecurityHeaders();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapOpenApi("/openapi/v1/openapi.json");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
            });
        }

        return app;
    }
}
