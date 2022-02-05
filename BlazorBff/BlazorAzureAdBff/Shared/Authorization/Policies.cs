using Microsoft.AspNetCore.Authorization;

namespace BlazorAzureADWithApis.Shared.Authorization
{
    public static class Policies
    {
        public const string DemoAdminsIdentifier = "demo-admins";
        public const string DemoAdminsValue = "1";

        public const string DemoUsersIdentifier = "demo-users";
        public const string DemoUsersValue = "1";

        public static AuthorizationPolicy DemoAdminsPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(DemoAdminsIdentifier, DemoAdminsValue)
                .Build();
        }

        public static AuthorizationPolicy DemoUsersPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(DemoUsersIdentifier, DemoUsersValue)
                .Build();
        }
    }
}
