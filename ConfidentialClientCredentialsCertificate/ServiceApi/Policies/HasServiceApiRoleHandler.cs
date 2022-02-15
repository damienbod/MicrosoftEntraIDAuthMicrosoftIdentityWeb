﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceApi;

public class HasServiceApiRoleHandler : AuthorizationHandler<HasServiceApiRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasServiceApiRoleRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var roleClaims = context.User.Claims.Where(t => t.Type == "roles");

        if (roleClaims != null && HasServiceApiRole(roleClaims))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private bool HasServiceApiRole(IEnumerable<Claim> roleClaims)
    {
        foreach (var role in roleClaims)
        {
            if ("service-api" == role.Value)
            {
                return true;
            }
        }

        return false;
    }
}