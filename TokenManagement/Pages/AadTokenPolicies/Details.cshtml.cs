﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using TokenManagement.AadTokenPolicies;

namespace TokenManagement.Pages.AadTokenPolicies;

[AuthorizeForScopes(Scopes = ["Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration", "Application.ReadWrite.All"])]
public class DetailsModel : PageModel
{
    private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

    public DetailsModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
    {
        _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
    }

    public TokenLifetimePolicyDto TokenLifetimePolicyDto { get; set; } = new();
    public List<PolicyAssignedApplicationsDto> PolicyAssignedApplications { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(id);
        TokenLifetimePolicyDto = new TokenLifetimePolicyDto
        {
            Definition = policy.Definition.FirstOrDefault()!,
            DisplayName = policy.DisplayName,
            IsOrganizationDefault = policy.IsOrganizationDefault.GetValueOrDefault(),
            Id = policy.Id
        };

        if (TokenLifetimePolicyDto == null)
        {
            return NotFound();
        }

        var applications = await _tokenLifetimePolicyGraphApiService.PolicyAppliesTo(id);
        PolicyAssignedApplications = applications.CurrentPage.Select(app => new PolicyAssignedApplicationsDto
        {
            Id = app.Id,
            DisplayName = (app as Microsoft.Graph.Application)!.DisplayName,
            AppId = (app as Microsoft.Graph.Application)!.AppId,
            SignInAudience = (app as Microsoft.Graph.Application)!.SignInAudience

        }).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string? appId = Request.Form["item.AppId"]!;
        string? policyId = Request.Form["TokenLifetimePolicyDto.Id"]!;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _tokenLifetimePolicyGraphApiService
            .RemovePolicyFromApplication(appId, policyId);

        return Redirect($"./Details?id={policyId}");
    }
}