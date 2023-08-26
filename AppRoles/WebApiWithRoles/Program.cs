
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
// By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
// 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles'
// This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
// JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.RoleClaimType = "roles";
        options.TokenValidationParameters.NameClaimType = "name";
    });

builder.Services.AddSwaggerGen(c =>
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
                {securityScheme, Array.Empty<string>()}
            });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Web API with roles",
        Version = "v1",
        Description = "Web API with roles",
        Contact = new OpenApiContact
        {
            Name = "damienbod",
            Email = string.Empty,
            Url = new Uri("https://damienbod.com/"),
        },
    });
});

builder.Services.AddAuthorization(policies =>
{
    policies.AddPolicy("p-web-api-with-roles-user", p =>
    {
        p.RequireClaim("roles", "web-api-with-roles-user");
    });
    policies.AddPolicy("p-web-api-with-roles-student", p =>
    {
        p.RequireClaim("roles", "web-api-with-roles-student");
    });
    policies.AddPolicy("p-web-api-with-roles-admin", p =>
    {
        p.RequireClaim("roles", "web-api-with-roles-admin");
    });

    policies.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
    {
        validateAccessTokenPolicy.RequireClaim("scp", "access_as_user");

        // Validate id of application for which the token was created
        // In this case the UI application 
        validateAccessTokenPolicy.RequireClaim("azp", "5c201b60-89f6-47d8-b2ef-9d9fe2a42751");

        // only allow tokens which used "Private key JWT Client authentication"
        // // https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens
        // Indicates how the client was authenticated. For a public client, the value is "0". 
        // If client ID and client secret are used, the value is "1". 
        // If a client certificate was used for authentication, the value is "2".
        validateAccessTokenPolicy.RequireClaim("azpacr", "1");
    });
});

builder.Services.AddControllers(options =>
{
    // global
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        // .RequireClaim("email") // disabled this to test with users that have no email (no license added)
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
// IdentityModelEventSource.ShowPII = true;

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API with roles");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
