using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenManagement.Pages
{
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
        public List<PolicyAssignedApplicationsDto> AllApplications { get; set; }

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

            var singleOrgApplications = await _tokenLifetimePolicyGraphApiService.GetApplicationsSingleOrMultipleOrg();
            
            ApplicationOptions = singleOrgApplications.CurrentPage.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id,
                                      Text = $"AppId: {a.AppId}, {a.DisplayName}"
                                  }).ToList();

            var allApplications = await _tokenLifetimePolicyGraphApiService.GetApplications();
            AllApplications = allApplications.CurrentPage.Select(app => new PolicyAssignedApplicationsDto
            {
                Id = app.Id,
                DisplayName = (app as Microsoft.Graph.Application).DisplayName,
                AppId = (app as Microsoft.Graph.Application).AppId,
                SignInAudience = (app as Microsoft.Graph.Application).SignInAudience

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
