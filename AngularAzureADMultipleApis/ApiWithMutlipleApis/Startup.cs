using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApiWithMutlipleApis.Services;

namespace ApiWithMutlipleApis;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddOptions();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        IdentityModelEventSource.ShowPII = true;
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

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

        services.AddMicrosoftIdentityWebApiAuthentication(Configuration)
             .EnableTokenAcquisitionToCallDownstreamApi()
             .AddMicrosoftGraph("https://graph.microsoft.com/beta", "User.ReadBasic.All user.read")
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

        services.AddSwaggerGen(c =>
        {
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
                {securityScheme, new string[] { }}
            });

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiWithMutlipleApis", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiWithMutlipleApis API");
            c.RoutePrefix = string.Empty;
        });

        app.UseCors("AllowAllOrigins");

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
