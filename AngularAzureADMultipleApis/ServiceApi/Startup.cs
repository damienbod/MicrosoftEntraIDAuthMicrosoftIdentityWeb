using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

namespace ServiceApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddSingleton<IAuthorizationHandler, HasServiceApiRoleHandler>();

            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);

            services.AddControllers();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
                {
                    validateAccessTokenPolicy.Requirements.Add(new HasServiceApiRoleRequirement());

                    // Validate id of application for which the token was created
                    // In this case the UI application 
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

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Service API One",
                    Version = "v1",
                    Description = "User API One",
                    Contact = new OpenApiContact
                    {
                        Name = "damienbod",
                        Email = string.Empty,
                        Url = new Uri("https://damienbod.com/"),
                    },
                });
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service API One");
                c.RoutePrefix = string.Empty;
            });

            // https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/
            // https://nblumhardt.com/2019/10/serilog-mvc-logging/
            app.UseSerilogRequestLogging();

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
}
