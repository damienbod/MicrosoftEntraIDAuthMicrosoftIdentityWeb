using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using Serilog;
using System;

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

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            // add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Service API One",
                Version = "v1",
                Description = "Service API One",
                Contact = new OpenApiContact
                {
                    Name = "damienbod",
                    Email = string.Empty,
                    Url = new Uri("https://damienbod.com/"),
                },
            });
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

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service API One");
            c.RoutePrefix = string.Empty;
        });

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
