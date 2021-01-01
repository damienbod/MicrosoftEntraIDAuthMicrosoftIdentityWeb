using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

namespace TokenManagement.Pages.AadTokenPolicies
{
    public class CreateModel : PageModel
    {
        private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

        public CreateModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
        {
            _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public TokenLifetimePolicyDto TokenLifetimePolicyDto { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tokenLifetimePolicy = new TokenLifetimePolicy
            {
                Definition = new List<string>()
                {
                    TokenLifetimePolicyDto.Definition
                },
                DisplayName = TokenLifetimePolicyDto.DisplayName,
                IsOrganizationDefault = TokenLifetimePolicyDto.IsOrganizationDefault,
                Description = TokenLifetimePolicyDto.Description
            };

            await _tokenLifetimePolicyGraphApiService.CreatePolicy(tokenLifetimePolicy);

            return RedirectToPage("./Index");
        }
    }
}
