using BlazorAzureADWithApis.Server.Services.Application;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorAzureADWithApis.Server
{
    public class GraphApiClaimsTransformation : IClaimsTransformation
    {
        private readonly MicrosoftGraphApplicationClient _microsoftGraphApplicationClient;

        public GraphApiClaimsTransformation(MicrosoftGraphApplicationClient microsoftGraphApplicationClient)
        {
            _microsoftGraphApplicationClient = microsoftGraphApplicationClient;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new();
            var groupClaimType = "group";
            if (!principal.HasClaim(claim => claim.Type == groupClaimType))
            {
                var objectidentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
                var objectIdentifier = principal.Claims.FirstOrDefault(t => t.Type == objectidentifierClaimType);

                var groupIds = await _microsoftGraphApplicationClient
                    .GetGraphUserMemberGroups(objectIdentifier.Value);

                foreach (var groupId in groupIds.ToList())
                {
                    claimsIdentity.AddClaim(new Claim(groupClaimType, groupId));
                }
            }

            principal.AddIdentity(claimsIdentity);
            return principal;
        }


    }
}
