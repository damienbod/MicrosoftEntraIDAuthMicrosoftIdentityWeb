using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using UserApiOne;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<UserApiTwoService>();
builder.Services.AddHttpClient();
builder.Services.AddOptions();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDistributedTokenCaches();

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

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        // .RequireClaim("email") // disabled this to test with users that have no email (no license added)
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
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
