using BlazorAzureADWithApis.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAzureADWithApis.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
            builder.Services.TryAddSingleton(sp => (HostAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
            builder.Services.AddTransient<AuthorizedHandler>();
            builder.Services.AddHttpClient("default", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddHttpClient("authorizedClient")
                .AddHttpMessageHandler<AuthorizedHandler>();
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));

            builder.RootComponents.Add<App>("#app");

            await builder.Build().RunAsync();
        }
    }
}
