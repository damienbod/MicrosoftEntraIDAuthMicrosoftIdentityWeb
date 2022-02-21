using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DeviceFlowWeb;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<DeviceFlowService>();
        services.AddScoped<AuthenticationSignInService>();
        services.AddHttpClient();
        services.Configure<AzureAdConfiguration>(Configuration.GetSection("AzureAd"));

        services.AddDistributedMemoryCache();

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
            options.Cookie.Name = "__Host-X-XSRF-TOKEN";
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name = "__Host-SESSION";
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie();

        services.AddAuthorization();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
        }

        //Registered before static files to always set header
           
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(opts => opts.NoReferrer());
        app.UseXXssProtection(options => options.EnabledWithBlockMode());
        app.UseXfo(options => options.Deny());

        //app.UseCsp(opts => opts
        //    .BlockAllMixedContent()
        //    .StyleSources(s => s.Self())
        //    .StyleSources(s => s.UnsafeInline())
        //    .FontSources(s => s.Self())
        //    .FormActions(s => s.Self())
        //    .FrameAncestors(s => s.Self())
        //    .ImageSources(imageSrc => imageSrc.Self())
        //    .ImageSources(imageSrc => imageSrc.CustomSources("data:"))
        //    .ScriptSources(s => s.Self())
        //);

        app.UseStaticFiles();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }
}