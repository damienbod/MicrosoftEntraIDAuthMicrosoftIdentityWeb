using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace TokenManagement.Pages
{
    [AuthorizeForScopes(Scopes = new string[] { "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration", "Application.ReadWrite.All" })]
    public class ApplicationsModel : PageModel
    {
        private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

        public ApplicationsModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
        {
            _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
        }

        public List<PolicyAssignedApplicationsDto> AllApplications { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            var allApplications = await _tokenLifetimePolicyGraphApiService.GetApplications();
            AllApplications = allApplications.CurrentPage.Select(app => new PolicyAssignedApplicationsDto
            {
                Id = app.Id,
                DisplayName = app.DisplayName,
                AppId = app.AppId,
                SignInAudience = app.SignInAudience,
                PolicyAssigned = GetFirstTokenLifetimePolicy(app.TokenLifetimePolicies)

            }).ToList();

            return Page();
        }

        private string GetFirstTokenLifetimePolicy(Microsoft.Graph.IApplicationTokenLifetimePoliciesCollectionWithReferencesPage tokenLifetimePolicies)
        {
            if (tokenLifetimePolicies == null)
            {
                return string.Empty;
            }

            if (tokenLifetimePolicies.Count == 0)
            {
                return string.Empty;
            }

            return tokenLifetimePolicies.First().DisplayName;
        }
    }
}
