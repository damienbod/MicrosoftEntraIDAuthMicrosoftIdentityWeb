using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace TokenManagement.Pages.AadTokenPolicies
{
    [AuthorizeForScopes(Scopes = new string[] { "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" })]
    public class EditModel : PageModel
    {
        private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

        public EditModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
        {
            _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
        }

        [BindProperty]
        public TokenLifetimePolicyDto TokenLifetimePolicyDto { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(id).ConfigureAwait(false);
            TokenLifetimePolicyDto = new TokenLifetimePolicyDto
            {
                Definition = policy.Definition.FirstOrDefault(),
                DisplayName = policy.DisplayName,
                IsOrganizationDefault = policy.IsOrganizationDefault.GetValueOrDefault(),
                Id = policy.Id
            };

            if (TokenLifetimePolicyDto == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // get existing
            var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(TokenLifetimePolicyDto.Id).ConfigureAwait(false);
            var tokenLifetimePolicy = new TokenLifetimePolicy
            {
                Id = TokenLifetimePolicyDto.Id,
                Definition = new List<string>()
                {
                    TokenLifetimePolicyDto.Definition
                },
                DisplayName = TokenLifetimePolicyDto.DisplayName,
                IsOrganizationDefault = TokenLifetimePolicyDto.IsOrganizationDefault,
            };


            await _tokenLifetimePolicyGraphApiService.UpdatePolicy(tokenLifetimePolicy).ConfigureAwait(false);

            return RedirectToPage("./Index");
        }
    }
}
