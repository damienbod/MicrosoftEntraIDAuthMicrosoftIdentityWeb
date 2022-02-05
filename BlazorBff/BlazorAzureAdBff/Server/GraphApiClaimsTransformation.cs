using BlazorAzureADWithApis.Server.Services.Application;
using BlazorAzureADWithApis.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorAzureADWithApis.Server
{
    public class GraphApiClaimsTransformation : IClaimsTransformation
    {
        private readonly MsGraphApplicationService _msGraphApplicationService;

        public GraphApiClaimsTransformation(MsGraphApplicationService msGraphApplicationService)
        {
            _msGraphApplicationService = msGraphApplicationService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new();
            var groupClaimType = "group";
            if (!principal.HasClaim(claim => claim.Type == groupClaimType))
            {
                var objectidentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
                var objectIdentifier = principal.Claims.FirstOrDefault(t => t.Type == objectidentifierClaimType);

                var groupIds = await _msGraphApplicationService
                    .GetGraphUserMemberGroups(objectIdentifier.Value);

                foreach (var groupId in groupIds.ToList())
                {
                    var claim = GetGroupClaim(groupId);
                    if (claim != null) claimsIdentity.AddClaim(claim);
                }
            }

            principal.AddIdentity(claimsIdentity);
            return principal;
        }

        private Claim GetGroupClaim(string groupId)
        {
            Claim claim = groupId switch
            {
                "1d9fba7e-b98a-45ec-b576-7ee77366cf10" => new Claim(Constants.DemoUsersIdentifier, Constants.DemoUsersValue),
                "be30f1dd-39c9-457b-ab22-55f5b67fb566" => new Claim(Constants.DemoAdminsIdentifier, Constants.DemoAdminsValue),
                _ => null
            };

            return claim;
        }
    }
}
