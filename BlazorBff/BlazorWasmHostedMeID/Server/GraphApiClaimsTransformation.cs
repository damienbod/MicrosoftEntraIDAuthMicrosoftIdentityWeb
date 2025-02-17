﻿using BlazorWasmHostedMeID.Server.Services.Application;
using BlazorWasmHostedMeID.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace BlazorWasmHostedMeID.Server;

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

            if (objectIdentifier != null)
            {
                var groupIds = await _msGraphApplicationService
                    .GetGraphApiUserMemberGroups(objectIdentifier.Value);

                foreach (var groupId in groupIds!.Value!.ToList())
                {
                    var claim = GetGroupClaim(groupId);
                    if (claim != null) claimsIdentity.AddClaim(claim);
                }
            }
        }

        principal.AddIdentity(claimsIdentity);
        return principal;
    }

    private static Claim? GetGroupClaim(string groupId)
    {
        Dictionary<string, Claim> mappings = new() {
            { "1d9fba7e-b98a-45ec-b576-7ee77366cf10",
                new Claim(Policies.DemoUsersIdentifier, Policies.DemoUsersValue)},

            { "be30f1dd-39c9-457b-ab22-55f5b67fb566",
                new Claim(Policies.DemoAdminsIdentifier, Policies.DemoAdminsValue)},
        };

        if (mappings.ContainsKey(groupId))
        {
            return mappings[groupId];
        }

        return null;
    }
}
