using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenManagement.Pages
{
    [AuthorizeForScopes(Scopes = new string[] { "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration", "Application.ReadWrite.All" })]
    public class AssignNewApplicationToPolicyModel : PageModel
    {
        private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

        public AssignNewApplicationToPolicyModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
        {
            _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
        }

        public TokenLifetimePolicyDto TokenLifetimePolicyDto { get; set; }

        public string ApplicationGraphId { get; set; }
        public List<SelectListItem> ApplicationOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(id);
            TokenLifetimePolicyDto = new TokenLifetimePolicyDto
            {
                Definition = policy.Definition.FirstOrDefault(),
                DisplayName = policy.DisplayName,
                IsOrganizationDefault = policy.IsOrganizationDefault.GetValueOrDefault(),
                Id = policy.Id
            };

            var singleAndMultipleOrgApplications = await _tokenLifetimePolicyGraphApiService.GetApplicationsSingleOrMultipleOrg();
            
            ApplicationOptions = singleAndMultipleOrgApplications.CurrentPage
                .Where(app => app.TokenLifetimePolicies != null && app.TokenLifetimePolicies.Count <=0)
                .Select(a =>
                    new SelectListItem
                    {
                        Value = a.Id,
                        Text = $"{a.DisplayName}" // AppId: {a.AppId}, 
                    }).ToList();

            if (TokenLifetimePolicyDto == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var applicationGraphId = Request.Form["ApplicationGraphId"];
            var policyId = Request.Form["TokenLifetimePolicyDto.Id"];
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _tokenLifetimePolicyGraphApiService
                .AssignTokenPolicyToApplicationUsingGraphId(applicationGraphId, policyId);

            return Redirect($"./Details?id={policyId}");
        }
    }
}
