using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;
using UserApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
    {
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

builder.Services.AddOpenApi(options =>
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

var app = builder.Build();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
IdentityModelEventSource.ShowPII = true;

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

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

app.Run();
