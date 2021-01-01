using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TokenManagement.Pages.AadTokenPolicies
{
    public class DetailsModel : PageModel
    {
        private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

        public DetailsModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
        {
            _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
        }

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
    }
}
