using ApiWithMutlipleApis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;

namespace ApiWithMutlipleApis;

internal static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddHttpClient();
        services.AddOptions();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .WithOrigins(
                            "https://localhost:4200")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddScoped<GraphApiClientService>();
        services.AddScoped<ServiceApiClientService>();
        services.AddScoped<UserApiClientService>();

        services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)
             .EnableTokenAcquisitionToCallDownstreamApi()
             .AddMicrosoftGraph()
             .AddInMemoryTokenCaches();

        services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
            {
                // Validate ClientId from token
                // only accept tokens issued ....
                validateAccessTokenPolicy.RequireClaim("azp", "ad6b0351-92b4-4ee9-ac8d-3e76e5fd1c67");
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

        app.UseCors("AllowAllOrigins");

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
