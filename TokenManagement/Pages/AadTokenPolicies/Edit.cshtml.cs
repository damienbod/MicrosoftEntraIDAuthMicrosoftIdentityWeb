using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TokenManagement.Pages.AadTokenPolicies
{
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

            var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(id);
            TokenLifetimePolicyDto = new TokenLifetimePolicyDto
            {
                Definition = policy.Definition.FirstOrDefault(),
                Description = policy.Description,
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

            var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(TokenLifetimePolicyDto.Id);
            policy.IsOrganizationDefault = TokenLifetimePolicyDto.IsOrganizationDefault;
            policy.Definition = new List<string>
            {
                TokenLifetimePolicyDto.Definition
            };
            policy.DisplayName = TokenLifetimePolicyDto.DisplayName;
            policy.Description = TokenLifetimePolicyDto.Description;

            await _tokenLifetimePolicyGraphApiService.UpdatePolicy(policy);

            return RedirectToPage("./Index");
        }
    }
}
