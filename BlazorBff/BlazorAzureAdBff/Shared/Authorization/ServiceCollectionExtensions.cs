using BlazorAzureADWithApis.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthz(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthorizationCore(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;

                options.AddPolicy("DemoAdmins", policy=>
                {
                    policy.RequireClaim(Constants.DemoAdminsIdentifier, Constants.DemoAdminsValue);
                });

                options.AddPolicy("DemoUsers", policy =>
                {
                    policy.RequireClaim(Constants.DemoUsersIdentifier, Constants.DemoUsersValue);
                });
            });

            return serviceCollection;
        }
    }
}
